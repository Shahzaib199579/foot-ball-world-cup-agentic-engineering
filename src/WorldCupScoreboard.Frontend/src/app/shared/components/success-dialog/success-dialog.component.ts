import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface SuccessDialogData {
  title?: string;
  message: string;
}

@Component({
  selector: 'app-success-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="success-dialog-container">
      <div class="success-dialog-header">
        <mat-icon class="success-icon">check_circle</mat-icon>
        <h2 mat-dialog-title>{{ data.title || 'Success' }}</h2>
      </div>
      <mat-dialog-content class="success-dialog-content">
        <p class="success-message">{{ data.message }}</p>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-flat-button color="primary" class="ok-btn" (click)="close()">OK</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .success-dialog-container {
      padding: 8px 4px;
    }
    .success-dialog-header {
      display: flex;
      align-items: center;
      gap: 12px;
      color: #15803d;
      padding-bottom: 8px;
    }
    .success-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
    }
    .success-dialog-header h2 {
      margin: 0;
      font-size: 1.3rem;
      font-weight: 600;
      color: #0a369d;
    }
    .success-dialog-content {
      margin-top: 8px;
      font-size: 0.95rem;
      color: #333;
    }
    .success-message {
      margin: 0;
      line-height: 1.5;
    }
    .ok-btn {
      background-color: #0a369d !important;
      color: #ffffff !important;
      border-radius: 6px;
      font-weight: 500;
    }
  `]
})
export class SuccessDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<SuccessDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SuccessDialogData
  ) {}

  close(): void {
    this.dialogRef.close();
  }
}
