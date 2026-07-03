import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { TrainingPlanService } from '../../../core/services/training-plan.service';
import { TrainingPlan, TrainingType, TrainingPlanStatus } from '../../../core/models/training-plan.model';

@Component({
  selector: 'app-my-plans',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './my-plans.html',
  styleUrl: './my-plans.css'
})
export class MyPlans implements OnInit {
  private planService = inject(TrainingPlanService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  plans = signal<TrainingPlan[]>([]);
  loading = signal(true);

  TrainingType = TrainingType;
  TrainingPlanStatus = TrainingPlanStatus;

  ngOnInit(): void {
    this.loadPlans();
  }

  loadPlans(): void {
    this.loading.set(true);
    this.planService.getMine().subscribe({
      next: (data) => {
        this.plans.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  createPlan(): void {
    this.router.navigate(['/trainer/plans/new']);
  }

  editPlan(plan: TrainingPlan): void {
    this.router.navigate(['/trainer/plans', plan.id, 'edit']);
  }

  deletePlan(plan: TrainingPlan): void {
    if (!confirm(`Da li sigurno zelite da obrisete plan "${plan.title}"?`)) {
      return;
    }

    this.planService.delete(plan.id).subscribe({
      next: () => {
        this.snackBar.open('Plan je obrisan.', 'OK', { duration: 3000 });
        this.loadPlans();
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Greska pri brisanju.', 'OK', { duration: 4000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/trainer']);
  }

  typeLabel(type: TrainingType): string {
    return type === TrainingType.Individual ? 'Individualni' : 'Grupni';
  }

  statusLabel(status: TrainingPlanStatus): string {
    return status === TrainingPlanStatus.Active ? 'Aktivan' : 'Arhiviran';
  }
  viewSessions(plan: TrainingPlan): void {
  this.router.navigate(['/trainer/plans', plan.id, 'sessions']);
}
}