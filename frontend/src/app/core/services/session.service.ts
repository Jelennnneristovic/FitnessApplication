import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TrainingSession, CreateSessionRequest } from '../models/session.model';

@Injectable({
  providedIn: 'root'
})
export class SessionService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Sesije za plan
  getByPlan(planId: string): Observable<TrainingSession[]> {
    return this.http.get<TrainingSession[]>(`${this.apiUrl}/api/sessions/by-plan/${planId}`);
  }

  // Trenerov raspored
  getTrainerSchedule(from?: string, to?: string): Observable<TrainingSession[]> {
    let params = new HttpParams();
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);
    return this.http.get<TrainingSession[]>(`${this.apiUrl}/api/sessions/trainer-schedule`, { params });
  }

  // Klijentov raspored
  getClientSchedule(from?: string, to?: string): Observable<TrainingSession[]> {
    let params = new HttpParams();
    if (from) params = params.set('from', from);
    if (to) params = params.set('to', to);
    return this.http.get<TrainingSession[]>(`${this.apiUrl}/api/sessions/client-schedule`, { params });
  }

  create(request: CreateSessionRequest): Observable<TrainingSession> {
    return this.http.post<TrainingSession>(`${this.apiUrl}/api/sessions`, request);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/sessions/${id}`);
  }
}