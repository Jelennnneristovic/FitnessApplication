import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { SessionService } from '../../../core/services/session.service';
import { TrainingPlanService } from '../../../core/services/training-plan.service';
import { TrainingSession, TrainingSessionStatus } from '../../../core/models/session.model';
import { TrainingPlan } from '../../../core/models/training-plan.model';

@Component({
  selector: 'app-plan-sessions',
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './plan-sessions.html',
  styleUrl: './plan-sessions.css'
})
export class PlanSessions implements OnInit {
  private fb = inject(FormBuilder);
  private sessionService = inject(SessionService);
  private planService = inject(TrainingPlanService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  plan = signal<TrainingPlan | null>(null);
  sessions = signal<TrainingSession[]>([]);
  loading = signal(true);
  saving = signal(false);
  showForm = signal(false);

  planId = '';
  TrainingSessionStatus = TrainingSessionStatus;

  // Forma za novu sesiju (datetime-local input)
  form = this.fb.group({
    startTime: ['', [Validators.required]],
    endTime: ['', [Validators.required]],
    notes: ['']
  });

  ngOnInit(): void {
    this.planId = this.route.snapshot.paramMap.get('planId') || '';
    this.loadPlan();
    this.loadSessions();
  }

  loadPlan(): void {
    this.planService.getById(this.planId).subscribe({
      next: (p) => this.plan.set(p)
    });
  }

  loadSessions(): void {
    this.loading.set(true);
    this.sessionService.getByPlan(this.planId).subscribe({
      next: (data) => {
        // Sortiraj po vremenu pocetka
        const sorted = [...data].sort((a, b) =>
          new Date(a.startTime).getTime() - new Date(b.startTime).getTime()
        );
        this.sessions.set(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  toggleForm(): void {
    this.showForm.set(!this.showForm());
    if (!this.showForm()) {
      this.form.reset();
    }
  }

  createSession(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const v = this.form.value;
    // datetime-local daje "2026-06-25T18:00" - saljemo kao ISO
    const startTime = new Date(v.startTime!).toISOString();
    const endTime = new Date(v.endTime!).toISOString();

    // Provera da je kraj posle pocetka
    if (new Date(endTime) <= new Date(startTime)) {
      this.snackBar.open('Kraj mora biti posle pocetka.', 'OK', { duration: 3000 });
      return;
    }

    this.saving.set(true);
    this.sessionService.create({
      trainingPlanId: this.planId,
      startTime,
      endTime,
      notes: v.notes || undefined
    }).subscribe({
      next: () => {
        this.saving.set(false);
        this.snackBar.open('Sesija je kreirana.', 'OK', { duration: 3000 });
        this.form.reset();
        this.showForm.set(false);
        this.loadSessions();
      },
      error: (err) => {
        this.saving.set(false);
        this.snackBar.open(err.error?.message || 'Greska pri kreiranju sesije.', 'OK', { duration: 4000 });
      }
    });
  }

  deleteSession(session: TrainingSession): void {
    if (!confirm('Da li sigurno zelite da obrisete ovu sesiju?')) return;

    this.sessionService.delete(session.id).subscribe({
      next: () => {
        this.snackBar.open('Sesija je obrisana.', 'OK', { duration: 3000 });
        this.loadSessions();
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Greska pri brisanju.', 'OK', { duration: 4000 });
      }
    });
  }

    viewAttendance(session: TrainingSession): void {
    this.router.navigate(['/trainer/plans', this.planId, 'sessions', session.id, 'attendance']);
  }

  goBack(): void {
    this.router.navigate(['/trainer/plans']);
  }

  statusLabel(status: TrainingSessionStatus): string {
    switch (status) {
      case TrainingSessionStatus.Scheduled: return 'Zakazana';
      case TrainingSessionStatus.Completed: return 'Zavrsena';
      case TrainingSessionStatus.Cancelled: return 'Otkazana';
      default: return 'Nepoznato';
    }
  }

  statusClass(status: TrainingSessionStatus): string {
    switch (status) {
      case TrainingSessionStatus.Scheduled: return 'status-scheduled';
      case TrainingSessionStatus.Completed: return 'status-completed';
      case TrainingSessionStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }
}