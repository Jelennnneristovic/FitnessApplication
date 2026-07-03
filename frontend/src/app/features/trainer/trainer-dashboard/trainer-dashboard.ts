import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationBell } from '../../../shared/notification-bell/notification-bell';



@Component({
  selector: 'app-trainer-dashboard',
  imports: [RouterLink, MatCardModule, MatButtonModule, MatToolbarModule, NotificationBell],
  templateUrl: './trainer-dashboard.html',
  styleUrl: './trainer-dashboard.css'
})
export class TrainerDashboard {
  private authService = inject(AuthService);
  private router = inject(Router);
  user = this.authService.currentUser;

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}