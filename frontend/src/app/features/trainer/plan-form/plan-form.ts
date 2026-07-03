import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { TrainingPlanService } from '../../../core/services/training-plan.service';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';
import { TrainingType } from '../../../core/models/training-plan.model';

@Component({
  selector: 'app-plan-form',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './plan-form.html',
  styleUrl: './plan-form.css'
})
export class PlanForm implements OnInit {
  private fb = inject(FormBuilder);
  private planService = inject(TrainingPlanService);
  private categoryService = inject(CategoryService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  categories = signal<Category[]>([]);
  loading = signal(false);
  saving = signal(false);
  isEditMode = signal(false);
  planId: string | null = null;

  TrainingType = TrainingType;

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required]],
    categoryId: ['', [Validators.required]],
    type: [TrainingType.Individual, [Validators.required]],
    price: [0, [Validators.required, Validators.min(0)]],
    maxParticipants: [1, [Validators.required, Validators.min(1)]],
    durationMinutes: [60, [Validators.required, Validators.min(15)]],
    location: ['']
  });

  ngOnInit(): void {
    // Dohvati kategorije za dropdown
    this.categoryService.getAll(false).subscribe({
      next: (cats) => this.categories.set(cats)
    });

    // Proveri da li je edit mode (ima id u ruti)
    this.planId = this.route.snapshot.paramMap.get('id');
    if (this.planId) {
      this.isEditMode.set(true);
      this.loadPlan(this.planId);
    }
  }

  loadPlan(id: string): void {
    this.loading.set(true);
    this.planService.getById(id).subscribe({
      next: (plan) => {
        this.form.patchValue({
          title: plan.title,
          description: plan.description,
          categoryId: plan.categoryId,
          type: plan.type,
          price: plan.price,
          maxParticipants: plan.maxParticipants,
          durationMinutes: plan.durationMinutes,
          location: plan.location || ''
        });
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Greska pri ucitavanju plana.', 'OK', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    const v = this.form.value;
    const payload = {
      title: v.title!,
      description: v.description!,
      categoryId: v.categoryId!,
      type: v.type!,
      price: v.price!,
      maxParticipants: v.maxParticipants!,
      durationMinutes: v.durationMinutes!,
      location: v.location || undefined
    };

    const request = this.isEditMode()
      ? this.planService.update(this.planId!, payload)
      : this.planService.create(payload);

    request.subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open(
          this.isEditMode() ? 'Plan je azuriran.' : 'Plan je kreiran.',
          'OK',
          { duration: 3000 }
        );
        this.router.navigate(['/trainer/plans']);
      },
      error: (err) => {
        this.saving.set(false);
        this.snackBar.open(err.error?.message || 'Greska pri cuvanju.', 'OK', { duration: 4000 });
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/trainer/plans']);
  }
}