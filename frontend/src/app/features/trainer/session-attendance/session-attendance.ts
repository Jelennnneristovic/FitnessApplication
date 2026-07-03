import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { SessionService } from '../../../core/services/session.service';
import { EnrollmentService } from '../../../core/services/enrollment.service';
import { AttendanceService } from '../../../core/services/attendance.service';
import { TrainingSession } from '../../../core/models/session.model';
import { Enrollment, EnrollmentStatus } from '../../../core/models/enrollment.model';
import { Attendance } from '../../../core/models/attendance.model';

@Component({
  selector: 'app-session-attendance',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './session-attendance.html',
  styleUrl: './session-attendance.css'
})
export class SessionAttendance implements OnInit {
  private sessionService = inject(SessionService);
  private enrollmentService = inject(EnrollmentService);
  private attendanceService = inject(AttendanceService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  session = signal<TrainingSession | null>(null);
  approvedClients = signal<Enrollment[]>([]);
  attendances = signal<Attendance[]>([]);
  loading = signal(true);
  processingClientId = signal<string | null>(null);

  sessionId = '';
  planId = '';

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('sessionId') || '';
    this.planId = this.route.snapshot.paramMap.get('planId') || '';
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);

    // Dohvati sesiju
    this.sessionService.getByPlan(this.planId).subscribe({
      next: (sessions) => {
        const found = sessions.find(s => s.id === this.sessionId);
        this.session.set(found ?? null);
      }
    });

    // Dohvati Approved klijente za plan
    this.enrollmentService.getByPlan(this.planId, EnrollmentStatus.Approved).subscribe({
      next: (enrollments) => {
        this.approvedClients.set(enrollments);
      }
    });

    // Dohvati postojeca prisustva
    this.attendanceService.getBySession(this.sessionId).subscribe({
      next: (data) => {
        this.attendances.set(data);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  // Da li je klijent vec oznacen i kako
  getAttendance(clientId: string): Attendance | undefined {
    return this.attendances().find(a => a.clientId === clientId);
  }

  mark(clientId: string, attended: boolean): void {
    this.processingClientId.set(clientId);
    this.attendanceService.markByTrainer(this.sessionId, clientId, { attended }).subscribe({
      next: () => {
        this.processingClientId.set(null);
        this.snackBar.open(attended ? 'Oznaceno kao prisutan.' : 'Oznaceno kao odsutan.', 'OK', { duration: 2000 });
        // Osvezi prisustva
        this.attendanceService.getBySession(this.sessionId).subscribe({
          next: (data) => this.attendances.set(data)
        });
      },
      error: (err) => {
        this.processingClientId.set(null);
        this.snackBar.open(err.error?.message || 'Greska.', 'OK', { duration: 4000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/trainer/plans', this.planId, 'sessions']);
  }

  // Skraceni ID za prikaz (posto nemamo ime)
  shortId(id: string): string {
    return id.substring(0, 8).toUpperCase();
  }
}