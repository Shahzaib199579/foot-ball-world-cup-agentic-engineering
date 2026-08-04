import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { SuccessDialogComponent } from '../../shared/components/success-dialog/success-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class SuccessDialogService {
  constructor(private dialog: MatDialog) {}

  openSuccess(message: string, title?: string): void {
    this.dialog.open(SuccessDialogComponent, {
      width: '440px',
      data: { title: title || 'Success', message },
      panelClass: 'custom-success-dialog-panel'
    });
  }
}
