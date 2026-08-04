import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogComponent } from '../../shared/components/error-dialog/error-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ErrorDialogService {
  constructor(private dialog: MatDialog) {}

  openError(message: string, title?: string, code?: string): void {
    this.dialog.open(ErrorDialogComponent, {
      width: '440px',
      data: { title: title || 'Action Failed', message, code },
      panelClass: 'custom-error-dialog-panel'
    });
  }
}
