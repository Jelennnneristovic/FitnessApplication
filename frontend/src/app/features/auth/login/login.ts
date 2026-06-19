import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

// Angular Material
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  // Stanje za prikaz
  loading = signal(false);
  errorMessage = signal<string | null>(null);

  // Reaktivna forma sa validacijama
  loginForm = this.fb.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  onSubmit(): void {
    // Ako forma nije validna, ne radi nista
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    const { username, password } = this.loginForm.value;

    this.authService.login({
      username: username!,
      password: password!
    }).subscribe({
      next: () => {
        this.loading.set(false);
        // Posle uspesnog logina - idi na pocetnu (za sad /  - kasnije dashboard)
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.loading.set(false);
        // Backend salje poruku u err.error.message ili tekst
        this.errorMessage.set(
          err.error?.message || err.error || 'Pogresan username ili lozinka.'
        );
      }
    });
  }
}