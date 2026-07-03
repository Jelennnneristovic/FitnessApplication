import { Component, inject, OnInit, signal } from '@angular/core';
import { Location } from '@angular/common';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { NotificationService } from '../../core/services/notification.service';
import { Notification, NotificationType } from '../../core/models/notification.model';

@Component({
  selector: 'app-notifications',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './notifications.html',
  styleUrl: './notifications.css'
})
export class Notifications implements OnInit {
  private notificationService = inject(NotificationService);
  private location = inject(Location);
  private snackBar = inject(MatSnackBar);

  notifications = signal<Notification[]>([]);
  loading = signal(true);

  NotificationType = NotificationType;

  ngOnInit(): void {
    this.loadNotifications();
  }

  loadNotifications(): void {
    this.loading.set(true);
    this.notificationService.getMine().subscribe({
      next: (data) => {
        // Najnovije prvo
        const sorted = [...data].sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.notifications.set(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  markAsRead(notification: Notification): void {
    if (notification.isRead) return;

    this.notificationService.markAsRead(notification.id).subscribe({
      next: () => this.loadNotifications()
    });
  }

  markAllAsRead(): void {
    this.notificationService.markAllAsRead().subscribe({
      next: () => {
        this.snackBar.open('Sve notifikacije su oznacene kao procitane.', 'OK', { duration: 3000 });
        this.loadNotifications();
      }
    });
  }

  goBack(): void {
    this.location.back();  // vrati na prethodnu stranu (radi za bilo koju ulogu)
  }

  hasUnread(): boolean {
    return this.notifications().some(n => !n.isRead);
  }

  // CSS klasa po tipu
  typeClass(type: NotificationType): string {
    switch (type) {
      case NotificationType.Success: return 'type-success';
      case NotificationType.Warning: return 'type-warning';
      case NotificationType.Info: return 'type-info';
      default: return 'type-info';
    }
  }
}