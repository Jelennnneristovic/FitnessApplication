import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TrainerSearchResult, TrainerSearchFilters } from '../models/trainer.model';
import { TrainerProfile, UpdateTrainerProfileRequest } from '../models/trainer-profile.model';


@Injectable({
  providedIn: 'root'
})
export class TrainerService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  searchTrainers(filters: TrainerSearchFilters): Observable<TrainerSearchResult[]> {
    let params = new HttpParams();

    if (filters.keyword) params = params.set('keyword', filters.keyword);
    if (filters.specialization) params = params.set('specialization', filters.specialization);
    if (filters.minRating != null) params = params.set('minRating', filters.minRating.toString());
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);

    return this.http.get<TrainerSearchResult[]>(
      `${this.apiUrl}/api/users/trainers/search`,
      { params }
    );
  }

  // Dohvati trenerski profil
  getProfile(userId: string): Observable<TrainerProfile> {
    return this.http.get<TrainerProfile>(`${this.apiUrl}/api/users/${userId}/trainer-profile`);
  }

  // Trener azurira svoj profil
  updateMyProfile(request: UpdateTrainerProfileRequest): Observable<TrainerProfile> {
    return this.http.put<TrainerProfile>(`${this.apiUrl}/api/users/me/trainer-profile`, request);
  }
}