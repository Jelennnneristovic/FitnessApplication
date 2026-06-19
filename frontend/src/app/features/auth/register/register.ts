import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../../core/services/auth.service';
import { UserRole, UserGender } from '../../../core/models/user.model';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  errorMessage = signal<string | null>(null);
  successMessage = signal<string | null>(null);

  // Enumi dostupni u templejtu (za dropdown opcije)
  UserRole = UserRole;
  UserGender = UserGender;

  registerForm = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    dateOfBirth: [null as Date | null, [Validators.required]],
    gender: [UserGender.Male, [Validators.required]],
    location: ['', [Validators.required]],
    // Samo Client ili Trainer - admin se ne registruje
    role: [UserRole.Client, [Validators.required]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);
    this.successMessage.set(null);

    const v = this.registerForm.value;

    // Datum iz Date objekta u "YYYY-MM-DD" string format
    const dob = v.dateOfBirth!;
    const dateString = this.formatDate(dob);

    this.authService.register({
      username: v.username!,
      email: v.email!,
      password: v.password!,
      firstName: v.firstName!,
      lastName: v.lastName!,
      dateOfBirth: dateString,
      gender: v.gender!,
      location: v.location!,
      role: v.role!
    }).subscribe({
      next: (response) => {
        this.loading.set(false);

        // Ako je trener, ceka odobrenje - razlicita poruka
        if (v.role === UserRole.Trainer) {
          this.successMessage.set(
            'Registracija uspesna! Vas nalog ceka odobrenje administratora.'
          );
          // Trener ne moze odmah da se uloguje, pa ga vodimo na login posle par sekundi
          setTimeout(() => this.router.navigate(['/login']), 3000);
        } else {
          // Klijent je odmah aktivan - vec je ulogovan (token sacuvan)
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(
          err.error?.message || err.error || 'Greska pri registraciji.'
        );
      }
    });
  }

  // Pretvara Date u "YYYY-MM-DD" (backend ocekuje taj format za DateOnly)
  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}