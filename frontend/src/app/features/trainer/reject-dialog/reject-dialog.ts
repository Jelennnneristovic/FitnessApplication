import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

interface DialogData {
  planTitle: string;
}

@Component({
  selector: 'app-reject-dialog',
  imports: [FormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './reject-dialog.html',
  styleUrl: './reject-dialog.css'
})
export class RejectDialog {
  private dialogRef = inject(MatDialogRef<RejectDialog>);
  data = inject<DialogData>(MAT_DIALOG_DATA);

  rejectionReason = '';

  onConfirm(): void {
    // Vrati razlog (moze biti prazan - razlog je opcioni)
    this.dialogRef.close({ rejectionReason: this.rejectionReason.trim() || undefined });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }
}