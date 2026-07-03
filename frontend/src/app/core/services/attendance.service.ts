import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Attendance, MarkAttendanceRequest } from '../models/attendance.model';

@Injectable({
  providedIn: 'root'
})
export class AttendanceService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  markByTrainer(sessionId: string, clientId: string, request: MarkAttendanceRequest): Observable<Attendance> {
    return this.http.post<Attendance>(
      `${this.apiUrl}/api/attendance/sessions/${sessionId}/clients/${clientId}/mark`,
      request
    );
  }

  getBySession(sessionId: string): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(`${this.apiUrl}/api/attendance/sessions/${sessionId}`);
  }

  // Klijent: moja istorija prisustva
  getMyHistory(): Observable<Attendance[]> {
    return this.http.get<Attendance[]>(`${this.apiUrl}/api/attendance/my-history`);
  }
}