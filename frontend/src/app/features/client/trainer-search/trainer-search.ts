import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatToolbarModule } from '@angular/material/toolbar';

import { TrainerService } from '../../../core/services/trainer.service';
import { TrainerSearchResult } from '../../../core/models/trainer.model';

@Component({
  selector: 'app-trainer-search',
  imports: [
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatToolbarModule
  ],
  templateUrl: './trainer-search.html',
  styleUrl: './trainer-search.css'
})
export class TrainerSearch implements OnInit {
  private trainerService = inject(TrainerService);
  private router = inject(Router);

  trainers = signal<TrainerSearchResult[]>([]);
  loading = signal(false);

  // Filteri
  keyword = '';
  specialization = '';
  minRating: number | null = null;
  sortBy = 'rating';

  ngOnInit(): void {
    this.search();
  }

  search(): void {
    this.loading.set(true);
    this.trainerService.searchTrainers({
      keyword: this.keyword || undefined,
      specialization: this.specialization || undefined,
      minRating: this.minRating ?? undefined,
      sortBy: this.sortBy
    }).subscribe({
      next: (data) => {
        this.trainers.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  clearFilters(): void {
    this.keyword = '';
    this.specialization = '';
    this.minRating = null;
    this.sortBy = 'rating';
    this.search();
  }

  goBack(): void {
    this.router.navigate(['/client']);
  }

  // Pravi niz za prikaz zvezdica: pune, polu, prazne
  getStars(rating: number): string[] {
    const stars: string[] = [];
    const rounded = Math.round(rating * 2) / 2;  // zaokruzi na 0.5
    for (let i = 1; i <= 5; i++) {
      if (i <= rounded) stars.push('full');
      else if (i - 0.5 === rounded) stars.push('half');
      else stars.push('empty');
    }
    return stars;
  }
      viewProfile(trainerId: string): void {
      this.router.navigate(['/client/trainers', trainerId]);
    }
}