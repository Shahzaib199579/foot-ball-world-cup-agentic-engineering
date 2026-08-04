import { TestBed } from '@angular/core/testing';
import { MatDialog } from '@angular/material/dialog';
import { SuccessDialogService } from './success-dialog.service';
import { SuccessDialogComponent } from '../../shared/components/success-dialog/success-dialog.component';

describe('SuccessDialogService', () => {
  let service: SuccessDialogService;
  let dialogSpy: jasmine.SpyObj<MatDialog>;

  beforeEach(() => {
    dialogSpy = jasmine.createSpyObj('MatDialog', ['open']);
    TestBed.configureTestingModule({
      providers: [SuccessDialogService, { provide: MatDialog, useValue: dialogSpy }]
    });
    service = TestBed.inject(SuccessDialogService);
  });

  it('opens SuccessDialogComponent with the given message and title', () => {
    service.openSuccess('Match started successfully.', 'Match Started');

    expect(dialogSpy.open).toHaveBeenCalledWith(
      SuccessDialogComponent,
      jasmine.objectContaining({
        data: { title: 'Match Started', message: 'Match started successfully.' }
      })
    );
  });

  it('defaults the title to "Success" when none is provided', () => {
    service.openSuccess('Done.');

    const callArgs = dialogSpy.open.calls.mostRecent().args[1] as { data: { title: string } };
    expect(callArgs.data.title).toBe('Success');
  });
});
