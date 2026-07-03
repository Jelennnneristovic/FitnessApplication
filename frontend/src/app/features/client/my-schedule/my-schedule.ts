import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { SessionService } from '../../../core/services/session.service';
import { AttendanceService } from '../../../core/services/attendance.service';
import { TrainingSession, TrainingSessionStatus } from '../../../core/models/session.model';
import { Attendance } from '../../../core/models/attendance.model';

@Component({
  selector: 'app-my-schedule',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './my-schedule.html',
  styleUrl: './my-schedule.css'
})
export class MySchedule implements OnInit {
  private sessionService = inject(SessionService);
  private attendanceService = inject(AttendanceService);
  private router = inject(Router);

  sessions = signal<TrainingSession[]>([]);
  attendances = signal<Attendance[]>([]);
  loading = signal(true);

  TrainingSessionStatus = TrainingSessionStatus;

  ngOnInit(): void {
    this.loadSchedule();
  }

  loadSchedule(): void {
    this.loading.set(true);

    // Dohvati raspored I istoriju prisustva paralelno
    forkJoin({
      schedule: this.sessionService.getClientSchedule(),
      history: this.attendanceService.getMyHistory()
    }).subscribe({
      next: ({ schedule, history }) => {
        const sorted = [...schedule].sort((a, b) =>
          new Date(a.startTime).getTime() - new Date(b.startTime).getTime()
        );
        this.sessions.set(sorted);
        this.attendances.set(history);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  // Nadji zapis prisustva za sesiju (ako postoji)
  getAttendance(sessionId: string): Attendance | undefined {
    return this.attendances().find(a => a.trainingSessionId === sessionId);
  }

  goBack(): void {
    this.router.navigate(['/client']);
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

  isPast(session: TrainingSession): boolean {
    return new Date(session.startTime).getTime() < Date.now();
  }
}