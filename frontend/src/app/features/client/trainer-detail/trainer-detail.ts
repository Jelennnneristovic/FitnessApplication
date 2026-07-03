import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

import { TrainerService } from '../../../core/services/trainer.service';
import { TrainingPlanService } from '../../../core/services/training-plan.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { ReviewService } from '../../../core/services/review.service';
import { TrainerSearchResult } from '../../../core/models/trainer.model';
import { TrainingPlan, TrainingType } from '../../../core/models/training-plan.model';
import { Review } from '../../../core/models/review.model';
import { ReviewDialog } from '../review-dialog/review-dialog';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-trainer-detail',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    DatePipe 
  ],
  templateUrl: './trainer-detail.html',
  styleUrl: './trainer-detail.css'
})
export class TrainerDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private trainerService = inject(TrainerService);
  private planService = inject(TrainingPlanService);
  private enrollmentService = inject(EnrollmentService);
  private reviewService = inject(ReviewService);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  trainer = signal<TrainerSearchResult | null>(null);
  plans = signal<TrainingPlan[]>([]);
  reviews = signal<Review[]>([]);
  loading = signal(true);
  enrollingPlanId = signal<string | null>(null);

  TrainingType = TrainingType;
  trainerId = '';

  ngOnInit(): void {
    this.trainerId = this.route.snapshot.paramMap.get('id') || '';
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);

    this.trainerService.searchTrainers({}).subscribe({
      next: (trainers) => {
        const found = trainers.find(t => t.id === this.trainerId);
        this.trainer.set(found ?? null);
      }
    });

    this.planService.getAll({ trainerId: this.trainerId }).subscribe({
      next: (plans) => {
        this.plans.set(plans);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });

    this.loadReviews();
  }

  loadReviews(): void {
    this.reviewService.getTrainerReviews(this.trainerId).subscribe({
      next: (data) => this.reviews.set(data),
      error: () => {}
    });
  }

  enroll(plan: TrainingPlan): void {
    this.enrollingPlanId.set(plan.id);
    this.enrollmentService.requestEnrollment({ trainingPlanId: plan.id }).subscribe({
      next: () => {
        this.enrollingPlanId.set(null);
        this.snackBar.open('Zahtev za prijavu je poslat! Ceka odobrenje trenera.', 'OK', { duration: 4000 });
      },
      error: (err) => {
        this.enrollingPlanId.set(null);
        this.snackBar.open(err.error?.message || 'Greska pri prijavi.', 'OK', { duration: 4000 });
      }
    });
  }

  // === OCENJIVANJE ===
  openReviewDialog(): void {
    const t = this.trainer();
    if (!t) return;

    const dialogRef = this.dialog.open(ReviewDialog, {
      data: { trainerName: `${t.firstName} ${t.lastName}` }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.reviewService.createReview(this.trainerId, result).subscribe({
          next: () => {
            this.snackBar.open('Hvala na oceni!', 'OK', { duration: 3000 });
            this.loadReviews();  // osvezi listu ocena
          },
          error: (err) => {
            // Cross-service greska: nije trenirao sa trenerom
            this.snackBar.open(err.error?.message || 'Greska pri ocenjivanju.', 'OK', { duration: 5000 });
          }
        });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/client/trainers']);
  }

  typeLabel(type: TrainingType): string {
    return type === TrainingType.Individual ? 'Individualni' : 'Grupni';
  }

  // Niz zvezdica za prikaz ocene u listi
  getStarsArray(rating: number): boolean[] {
    return [1, 2, 3, 4, 5].map(i => i <= rating);
  }
}