import { Component, inject, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../../core/services/auth.service';
import { UserService } from '../../../core/services/user.service';
import { CategoryService } from '../../../core/services/category.service';

@Component({
  selector: 'app-admin-dashboard',
  imports: [
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {
  private authService = inject(AuthService);
  private userService = inject(UserService);
  private categoryService = inject(CategoryService);
  private router = inject(Router);

  user = this.authService.currentUser;

  // Statistike
  clientsCount = signal<number>(0);
  trainersCount = signal<number>(0);
  categoriesCount = signal<number>(0);
  loadingStats = signal(true);

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loadingStats.set(true);

    // Dohvati sve tri liste i prebroj
    this.userService.getClients().subscribe({
      next: (clients) => this.clientsCount.set(clients.length)
    });

    this.userService.getTrainers().subscribe({
      next: (trainers) => this.trainersCount.set(trainers.length)
    });

    this.categoryService.getAll(false).subscribe({
      next: (categories) => {
        this.categoriesCount.set(categories.length);
        this.loadingStats.set(false);
      },
      error: () => this.loadingStats.set(false)
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}