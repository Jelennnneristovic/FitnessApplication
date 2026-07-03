import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Review, CreateReviewRequest } from '../models/review.model';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Sve ocene trenera
  getTrainerReviews(trainerId: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.apiUrl}/api/trainers/${trainerId}/reviews`);
  }

  // Klijent ostavlja ocenu (cross-service provera na backendu)
  createReview(trainerId: string, request: CreateReviewRequest): Observable<Review> {
    return this.http.post<Review>(`${this.apiUrl}/api/trainers/${trainerId}/reviews`, request);
  }
}