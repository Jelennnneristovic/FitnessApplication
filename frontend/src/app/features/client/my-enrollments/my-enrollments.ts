import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';

import { EnrollmentService } from '../../../core/services/enrollment.service';
import { Enrollment, EnrollmentStatus } from '../../../core/models/enrollment.model';

@Component({
  selector: 'app-my-enrollments',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './my-enrollments.html',
  styleUrl: './my-enrollments.css'
})
export class MyEnrollments implements OnInit {
  private enrollmentService = inject(EnrollmentService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  enrollments = signal<Enrollment[]>([]);
  loading = signal(true);

  EnrollmentStatus = EnrollmentStatus;

  ngOnInit(): void {
    this.loadEnrollments();
  }

  loadEnrollments(): void {
    this.loading.set(true);
    this.enrollmentService.getMine().subscribe({
      next: (data) => {
        // Sortiraj: najnoviji prvo
        const sorted = [...data].sort((a, b) =>
          new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime()
        );
        this.enrollments.set(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  cancelEnrollment(enrollment: Enrollment): void {
    if (!confirm(`Da li sigurno zelite da otkazete zahtev za "${enrollment.trainingPlanTitle}"?`)) {
      return;
    }

    this.enrollmentService.cancel(enrollment.id).subscribe({
      next: () => {
        this.snackBar.open('Zahtev je otkazan.', 'OK', { duration: 3000 });
        this.loadEnrollments();
      },
      error: (err) => {
        this.snackBar.open(err.error?.message || 'Greska pri otkazivanju.', 'OK', { duration: 4000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/client']);
  }

  // Tekst statusa
  statusLabel(status: EnrollmentStatus): string {
    switch (status) {
      case EnrollmentStatus.Pending: return 'Ceka odgovor';
      case EnrollmentStatus.Approved: return 'Odobreno';
      case EnrollmentStatus.Rejected: return 'Odbijeno';
      case EnrollmentStatus.Cancelled: return 'Otkazano';
      default: return 'Nepoznato';
    }
  }

  // CSS klasa za boju statusa
  statusClass(status: EnrollmentStatus): string {
    switch (status) {
      case EnrollmentStatus.Pending: return 'status-pending';
      case EnrollmentStatus.Approved: return 'status-approved';
      case EnrollmentStatus.Rejected: return 'status-rejected';
      case EnrollmentStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }
}