import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Notification } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getMine(): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.apiUrl}/api/notifications/mine`);
  }

  getUnread(): Observable<Notification[]> {
    return this.http.get<Notification[]>(`${this.apiUrl}/api/notifications/mine/unread`);
  }

  markAsRead(id: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/api/notifications/${id}/read`, {});
  }

  markAllAsRead(): Observable<any> {
    return this.http.patch(`${this.apiUrl}/api/notifications/mark-all-read`, {});
  }
}