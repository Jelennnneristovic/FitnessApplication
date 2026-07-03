import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-category-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule
  ],
  templateUrl: './category-dialog.html',
  styleUrl: './category-dialog.css'
})
export class CategoryDialog {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CategoryDialog>);

  // Podaci koji se prosledjuju dijalogu (null = create, postojeca = edit)
  data = inject<Category | null>(MAT_DIALOG_DATA);

  isEditMode = this.data !== null;

  form = this.fb.group({
    name: [this.data?.name || '', [Validators.required, Validators.minLength(2)]],
    description: [this.data?.description || '']
  });

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    // Vrati podatke pozivaocu (lista komponenta ce ih iskoristiti)
    this.dialogRef.close({
      name: this.form.value.name,
      description: this.form.value.description
    });
  }

  onCancel(): void {
    this.dialogRef.close(null);  // zatvori bez podataka
  }
}