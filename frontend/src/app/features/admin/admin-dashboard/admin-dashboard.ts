import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-admin-dashboard',
  imports: [MatCardModule, MatButtonModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard {
  private authService = inject(AuthService);
  private router = inject(Router);
  user = this.authService.currentUser;

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}