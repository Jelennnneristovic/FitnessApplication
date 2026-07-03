import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';

import { EnrollmentService } from '../../../core/services/enrollment.service';
import { Enrollment, EnrollmentStatus } from '../../../core/models/enrollment.model';
import { RejectDialog } from '../reject-dialog/reject-dialog';

@Component({
  selector: 'app-trainer-enrollments',
  imports: [
    DatePipe,
    MatCardModule,
    MatButtonModule,
    MatToolbarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './trainer-enrollments.html',
  styleUrl: './trainer-enrollments.css'
})
export class TrainerEnrollments implements OnInit {
  private enrollmentService = inject(EnrollmentService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private dialog = inject(MatDialog);

  enrollments = signal<Enrollment[]>([]);
  loading = signal(true);
  processingId = signal<string | null>(null);

  EnrollmentStatus = EnrollmentStatus;

  ngOnInit(): void {
    this.loadEnrollments();
  }

  loadEnrollments(): void {
    this.loading.set(true);
    this.enrollmentService.getForMyPlans().subscribe({
      next: (data) => {
        // Pending prvo (da trener vidi sta ceka), pa ostali
        const sorted = [...data].sort((a, b) => {
          if (a.status === EnrollmentStatus.Pending && b.status !== EnrollmentStatus.Pending) return -1;
          if (a.status !== EnrollmentStatus.Pending && b.status === EnrollmentStatus.Pending) return 1;
          return new Date(b.requestedAt).getTime() - new Date(a.requestedAt).getTime();
        });
        this.enrollments.set(sorted);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  approve(enrollment: Enrollment): void {
    this.processingId.set(enrollment.id);
    this.enrollmentService.approve(enrollment.id).subscribe({
      next: () => {
        this.processingId.set(null);
        this.snackBar.open('Zahtev je odobren. Klijent je obavesten.', 'OK', { duration: 3000 });
        this.loadEnrollments();
      },
      error: (err) => {
        this.processingId.set(null);
        this.snackBar.open(err.error?.message || 'Greska pri odobravanju.', 'OK', { duration: 4000 });
      }
    });
  }

  openRejectDialog(enrollment: Enrollment): void {
    const dialogRef = this.dialog.open(RejectDialog, {
      data: { planTitle: enrollment.trainingPlanTitle }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {  // result = { rejectionReason }
        this.reject(enrollment, result.rejectionReason);
      }
    });
  }

  private reject(enrollment: Enrollment, reason?: string): void {
    this.processingId.set(enrollment.id);
    this.enrollmentService.reject(enrollment.id, reason).subscribe({
      next: () => {
        this.processingId.set(null);
        this.snackBar.open('Zahtev je odbijen. Klijent je obavesten.', 'OK', { duration: 3000 });
        this.loadEnrollments();
      },
      error: (err) => {
        this.processingId.set(null);
        this.snackBar.open(err.error?.message || 'Greska pri odbijanju.', 'OK', { duration: 4000 });
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/trainer']);
  }

  statusLabel(status: EnrollmentStatus): string {
    switch (status) {
      case EnrollmentStatus.Pending: return 'Ceka odgovor';
      case EnrollmentStatus.Approved: return 'Odobreno';
      case EnrollmentStatus.Rejected: return 'Odbijeno';
      case EnrollmentStatus.Cancelled: return 'Otkazano';
      default: return 'Nepoznato';
    }
  }

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