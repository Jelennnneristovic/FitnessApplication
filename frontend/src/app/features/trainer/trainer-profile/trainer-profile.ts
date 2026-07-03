import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { TrainerService } from '../../../core/services/trainer.service';
import { AuthService } from '../../../core/services/auth.service';
import { TrainerProfile as TrainerProfileModel } from '../../../core/models/trainer-profile.model';

@Component({
  selector: 'app-trainer-profile',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './trainer-profile.html',
  styleUrl: './trainer-profile.css'
})
export class TrainerProfile implements OnInit {
  private fb = inject(FormBuilder);
  private trainerService = inject(TrainerService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  profile = signal<TrainerProfileModel | null>(null);
  loading = signal(true);
  saving = signal(false);
  editMode = signal(false);

  form = this.fb.group({
    specialization: [''],
    yearsOfExperience: [null as number | null, [Validators.min(0), Validators.max(70)]],
    description: ['']
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    const userId = this.authService.currentUser()?.id;
    if (!userId) return;

    this.loading.set(true);
    this.trainerService.getProfile(userId).subscribe({
      next: (data) => {
        this.profile.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  startEdit(): void {
    const p = this.profile();
    this.form.patchValue({
      specialization: p?.specialization || '',
      yearsOfExperience: p?.yearsOfExperience ?? null,
      description: p?.description || ''
    });
    this.editMode.set(true);
  }

  cancelEdit(): void {
    this.editMode.set(false);
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const v = this.form.value;

    this.trainerService.updateMyProfile({
      specialization: v.specialization || undefined,
      yearsOfExperience: v.yearsOfExperience ?? undefined,
      description: v.description || undefined
    }).subscribe({
      next: (updated) => {
        this.saving.set(false);
        this.profile.set(updated);
        this.editMode.set(false);
        this.snackBar.open('Profil je azuriran.', 'OK', { duration: 3000 });
      },
      error: (err) => {
        this.saving.set(false);
        this.snackBar.open(err.error?.message || 'Greska pri cuvanju.', 'OK', { duration: 4000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/trainer']);
  }
}