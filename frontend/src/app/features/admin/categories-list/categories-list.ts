import { Component, inject, OnInit, signal } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';

import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';
import { CategoryDialog } from '../category-dialog/category-dialog';

@Component({
  selector: 'app-categories-list',
  imports: [
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    MatSlideToggleModule
  ],
  templateUrl: './categories-list.html',
  styleUrl: './categories-list.css'
})
export class CategoriesList implements OnInit {
  private categoryService = inject(CategoryService);
  private dialog = inject(MatDialog);

  categories = signal<Category[]>([]);
  loading = signal(false);
  showInactive = signal(false);

  displayedColumns = ['name', 'description', 'status', 'actions'];

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.loading.set(true);
    this.categoryService.getAll(this.showInactive())
      .subscribe({
        next: (data) => {
          this.categories.set(data);
          this.loading.set(false);
        },
        error: () => this.loading.set(false)
      });
  }

  toggleInactive(): void {
    this.showInactive.set(!this.showInactive());
    this.loadCategories();
  }

  // Otvori dijalog za NOVU kategoriju
  openCreateDialog(): void {
    const dialogRef = this.dialog.open(CategoryDialog, {
      data: null  // null = create mode
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.categoryService.create(result).subscribe({
          next: () => this.loadCategories(),
          error: (err) => alert(err.error?.message || 'Greska pri kreiranju.')
        });
      }
    });
  }

  // Otvori dijalog za IZMENU
  openEditDialog(category: Category): void {
    const dialogRef = this.dialog.open(CategoryDialog, {
      data: category  // postojeca = edit mode
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.categoryService.update(category.id, result).subscribe({
          next: () => this.loadCategories(),
          error: (err) => alert(err.error?.message || 'Greska pri izmeni.')
        });
      }
    });
  }

  // Obrisi (soft delete)
  deleteCategory(category: Category): void {
    if (!confirm(`Da li sigurno zelite da obrisete kategoriju "${category.name}"?`)) {
      return;
    }

    this.categoryService.delete(category.id).subscribe({
      next: () => this.loadCategories(),
      error: (err) => alert(err.error?.message || 'Greska pri brisanju.')
    });
  }
}