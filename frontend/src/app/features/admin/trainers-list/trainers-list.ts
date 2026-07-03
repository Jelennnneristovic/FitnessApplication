import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserService } from '../../../core/services/user.service';
import { UserListItem, UserStatus } from '../../../core/models/user.model';

@Component({
  selector: 'app-trainers-list',
  imports: [
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './trainers-list.html',
  styleUrl: './trainers-list.css'
})
export class TrainersList implements OnInit {
  private userService = inject(UserService);

  trainers = signal<UserListItem[]>([]);
  loading = signal(false);

  keyword = '';
  selectedStatus: UserStatus | null = null;

  UserStatus = UserStatus;
  displayedColumns = ['username', 'fullName', 'email', 'status', 'actions'];

  ngOnInit(): void {
    this.loadTrainers();
  }

  loadTrainers(): void {
    this.loading.set(true);
    this.userService.getTrainers(this.keyword, this.selectedStatus ?? undefined)
      .subscribe({
        next: (data) => {
          this.trainers.set(data);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  onSearch(): void {
    this.loadTrainers();
  }

  clearFilters(): void {
    this.keyword = '';
    this.selectedStatus = null;
    this.loadTrainers();
  }

  activate(userId: string): void {
    this.userService.activate(userId).subscribe({
      next: () => this.loadTrainers(),
      error: (err) => alert(err.error?.message || 'Greska pri aktivaciji.')
    });
  }

  deactivate(userId: string): void {
    this.userService.deactivate(userId).subscribe({
      next: () => this.loadTrainers(),
      error: (err) => alert(err.error?.message || 'Greska pri deaktivaciji.')
    });
  }

  statusLabel(status: UserStatus): string {
    switch (status) {
      case UserStatus.Active: return 'Aktivan';
      case UserStatus.InActive: return 'Neaktivan';
      case UserStatus.PendingApproval: return 'Ceka odobrenje';
      default: return 'Nepoznato';
    }
  }

  statusClass(status: UserStatus): string {
    switch (status) {
      case UserStatus.Active: return 'status-active';
      case UserStatus.InActive: return 'status-inactive';
      case UserStatus.PendingApproval: return 'status-pending';
      default: return '';
    }
  }
}