import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { UserService } from '../../../core/services/user.service';
import { UserListItem, UserStatus } from '../../../core/models/user.model';

@Component({
  selector: 'app-clients-list',
  imports: [
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './clients-list.html',
  styleUrl: './clients-list.css'
})
export class ClientsList implements OnInit {
  private userService = inject(UserService);

  clients = signal<UserListItem[]>([]);
  loading = signal(false);

  // Filter vrednosti
  keyword = '';
  selectedStatus: UserStatus | null = null;

  // Za dropdown i prikaz
  UserStatus = UserStatus;
  displayedColumns = ['username', 'fullName', 'email', 'status', 'actions'];

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading.set(true);
    this.userService.getClients(this.keyword, this.selectedStatus ?? undefined)
      .subscribe({
        next: (data) => {
          this.clients.set(data);
          this.loading.set(false);
        },
        error: () => {
          this.loading.set(false);
        }
      });
  }

  onSearch(): void {
    this.loadClients();
  }

  clearFilters(): void {
    this.keyword = '';
    this.selectedStatus = null;
    this.loadClients();
  }

  activate(userId: string): void {
    this.userService.activate(userId).subscribe({
      next: () => this.loadClients(),
      error: (err) => alert(err.error?.message || 'Greska pri aktivaciji.')
    });
  }

  deactivate(userId: string): void {
    this.userService.deactivate(userId).subscribe({
      next: () => this.loadClients(),
      error: (err) => alert(err.error?.message || 'Greska pri deaktivaciji.')
    });
  }

  // Helper za prikaz statusa kao tekst
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