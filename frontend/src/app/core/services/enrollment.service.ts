import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateEnrollmentRequest, Enrollment, EnrollmentStatus  } from '../models/enrollment.model';

@Injectable({
  providedIn: 'root'
})
export class EnrollmentService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // === KLIJENT ===
  requestEnrollment(request: CreateEnrollmentRequest): Observable<Enrollment> {
    return this.http.post<Enrollment>(`${this.apiUrl}/api/enrollments`, request);
  }

  getMine(): Observable<Enrollment[]> {
    return this.http.get<Enrollment[]>(`${this.apiUrl}/api/enrollments/mine`);
  }

  cancel(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/enrollments/${id}/cancel`);
  }

  // === TRENER ===
  getForMyPlans(status?: number): Observable<Enrollment[]> {
    let params = new HttpParams();
    if (status != null) params = params.set('status', status.toString());
    return this.http.get<Enrollment[]>(`${this.apiUrl}/api/enrollments/for-my-plans`, { params });
  }

  approve(id: string): Observable<Enrollment> {
    return this.http.patch<Enrollment>(`${this.apiUrl}/api/enrollments/${id}/approve`, {});
  }

  reject(id: string, rejectionReason?: string): Observable<Enrollment> {
    return this.http.patch<Enrollment>(`${this.apiUrl}/api/enrollments/${id}/reject`, { rejectionReason });
  }

    getByPlan(planId: string, status?: EnrollmentStatus): Observable<Enrollment[]> {
    let params = new HttpParams();
    if (status != null) params = params.set('status', status.toString());
    return this.http.get<Enrollment[]>(`${this.apiUrl}/api/enrollments/by-plan/${planId}`, { params });
  }
}