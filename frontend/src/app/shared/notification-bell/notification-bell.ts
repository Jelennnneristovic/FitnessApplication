import { Component, inject, OnInit, signal, input } from '@angular/core';
import { Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';

import { NotificationService } from '../../core/services/notification.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { EnrollmentStatus } from '../../core/models/enrollment.model';

@Component({
  selector: 'app-notification-bell',
  imports: [MatIconModule, MatBadgeModule, MatButtonModule],
  templateUrl: './notification-bell.html',
  styleUrl: './notification-bell.css'
})
export class NotificationBell implements OnInit {
  private notificationService = inject(NotificationService);
  private enrollmentService = inject(EnrollmentService);
  private router = inject(Router);

  // 'notifications' (klijent) ili 'requests' (trener)
  mode = input<'notifications' | 'requests'>('notifications');

  count = signal(0);

  ngOnInit(): void {
    this.loadCount();
  }

  loadCount(): void {
    if (this.mode() === 'requests') {
      // Trener: broj Pending zahteva
      this.enrollmentService.getForMyPlans(EnrollmentStatus.Pending).subscribe({
        next: (data) => this.count.set(data.length),
        error: () => this.count.set(0)
      });
    } else {
      // Klijent: broj nepročitanih notifikacija
      this.notificationService.getUnread().subscribe({
        next: (data) => this.count.set(data.length),
        error: () => this.count.set(0)
      });
    }
  }

  openTarget(): void {
    if (this.mode() === 'requests') {
      this.router.navigate(['/trainer/enrollments']);
    } else {
      this.router.navigate(['/notifications']);
    }
  }
}