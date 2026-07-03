import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TrainingPlan } from '../models/training-plan.model';


// Tip za create/update payload
export interface TrainingPlanPayload {
  categoryId?: string;
  title?: string;
  description?: string;
  type?: number;
  price?: number;
  maxParticipants?: number;
  durationMinutes?: number;
  location?: string;
  status?: number;
}

@Injectable({
  providedIn: 'root'
})
export class TrainingPlanService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Svi planovi sa opcionim filterima
  getAll(filters?: { trainerId?: string; categoryId?: string; type?: number; keyword?: string }): Observable<TrainingPlan[]> {
    let params = new HttpParams();
    if (filters?.trainerId) params = params.set('trainerId', filters.trainerId);
    if (filters?.categoryId) params = params.set('categoryId', filters.categoryId);
    if (filters?.type != null) params = params.set('type', filters.type.toString());
    if (filters?.keyword) params = params.set('keyword', filters.keyword);

    return this.http.get<TrainingPlan[]>(`${this.apiUrl}/api/training-plans`, { params });
  }

  getById(id: string): Observable<TrainingPlan> {
    return this.http.get<TrainingPlan>(`${this.apiUrl}/api/training-plans/${id}`);
  }
  // Trener: moji planovi
  getMine(): Observable<TrainingPlan[]> {
    return this.http.get<TrainingPlan[]>(`${this.apiUrl}/api/training-plans/mine`);
  }

  create(payload: TrainingPlanPayload): Observable<TrainingPlan> {
    return this.http.post<TrainingPlan>(`${this.apiUrl}/api/training-plans`, payload);
  }

  update(id: string, payload: TrainingPlanPayload): Observable<TrainingPlan> {
    return this.http.put<TrainingPlan>(`${this.apiUrl}/api/training-plans/${id}`, payload);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/training-plans/${id}`);
  }

}