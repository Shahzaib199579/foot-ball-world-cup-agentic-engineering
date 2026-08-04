import { TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { ErrorDialogService } from './error-dialog.service';
import { ErrorDialogComponent } from '../../shared/components/error-dialog/error-dialog.component';

describe('ErrorDialogService', () => {
  let service: ErrorDialogService;
  let dialogSpy: jasmine.SpyObj<MatDialog>;

  beforeEach(() => {
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    TestBed.configureTestingModule({
      providers: [ErrorDialogService, { provide: MatDialog, useValue: dialogSpy }]
    });
    service = TestBed.inject(ErrorDialogService);
  });

  it('opens ErrorDialogComponent with the given message, title, and code', () => {
    service.openError('Something went wrong', 'Oops', 'invalid_score');

    expect(dialogSpy.open).toHaveBeenCalledWith(
      ErrorDialogComponent,
      jasmine.objectContaining({
        data: { title: 'Oops', message: 'Something went wrong', code: 'invalid_score' }
      })
    );
  });

  it('defaults the title when none is provided', () => {
    service.openError('Something went wrong');

    const callArgs = dialogSpy.open.calls.mostRecent().args[1] as { data: { title: string } };
    expect(callArgs.data.title).toBe('Action Failed');
  });
});
