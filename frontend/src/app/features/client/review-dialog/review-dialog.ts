import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

interface DialogData {
  trainerName: string;
}

@Component({
  selector: 'app-review-dialog',
  imports: [
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './review-dialog.html',
  styleUrl: './review-dialog.css'
})
export class ReviewDialog {
  private dialogRef = inject(MatDialogRef<ReviewDialog>);
  data = inject<DialogData>(MAT_DIALOG_DATA);

  // Trenutno izabrana ocena (1-5)
  rating = signal(0);
  // Ocena preko koje je mis (hover efekat)
  hoverRating = signal(0);
  comment = '';

  // Niz [1,2,3,4,5] za prikaz zvezdica
  stars = [1, 2, 3, 4, 5];

  setRating(value: number): void {
    this.rating.set(value);
  }

  setHover(value: number): void {
    this.hoverRating.set(value);
  }

  clearHover(): void {
    this.hoverRating.set(0);
  }

  // Da li zvezdica treba da bude "puna"
  isStarFilled(star: number): boolean {
    const active = this.hoverRating() || this.rating();
    return star <= active;
  }

  onSave(): void {
    if (this.rating() === 0) {
      return;  // mora da izabere ocenu
    }

    this.dialogRef.close({
      rating: this.rating(),
      comment: this.comment.trim() || undefined
    });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}