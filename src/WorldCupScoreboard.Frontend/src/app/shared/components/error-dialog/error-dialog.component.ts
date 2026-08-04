import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface ErrorDialogData {
  title?: string;
  message: string;
  code?: string;
}

@Component({
  selector: 'app-error-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  template: `
    <div class="error-dialog-container">
      <div class="error-dialog-header">
        <mat-icon class="error-icon">error_outline</mat-icon>
        <h2 mat-dialog-title>{{ data.title || 'Error Occurred' }}</h2>
      </div>
      <mat-dialog-content class="error-dialog-content">
        <p class="error-message">{{ data.message }}</p>
        <p *ngIf="data.code" class="error-code">Code: <code>{{ data.code }}</code></p>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-flat-button color="primary" class="dismiss-btn" (click)="close()">Dismiss</button>
      </mat-dialog-actions>
    </div>
  `,
  styles: [`
    .error-dialog-container {
      padding: 8px 4px;
    }
    .error-dialog-header {
      display: flex;
      align-items: center;
      gap: 12px;
      color: #d32f2f;
      padding-bottom: 8px;
    }
    .error-icon {
      font-size: 32px;
      width: 32px;
      height: 32px;
    }
    .error-dialog-header h2 {
      margin: 0;
      font-size: 1.3rem;
      font-weight: 600;
      color: #0a369d;
    }
    .error-dialog-content {
      margin-top: 8px;
      font-size: 0.95rem;
      color: #333;
    }
    .error-message {
      margin: 0 0 12px 0;
      line-height: 1.5;
    }
    .error-code {
      font-size: 0.85rem;
      color: #666;
      margin: 0;
      code {
        background: #f1f3f5;
        padding: 2px 6px;
        border-radius: 4px;
        color: #d32f2f;
      }
    }
    .dismiss-btn {
      background-color: #0a369d !important;
      color: #ffffff !important;
      border-radius: 6px;
      font-weight: 500;
    }
  `]
})
export class ErrorDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ErrorDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ErrorDialogData
  ) {}

  close(): void {
    this.dialogRef.close();
  }
}
