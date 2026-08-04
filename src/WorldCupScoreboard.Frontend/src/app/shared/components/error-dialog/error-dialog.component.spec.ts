import { TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ErrorDialogComponent } from './error-dialog.component';

describe('ErrorDialogComponent', () => {
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<ErrorDialogComponent>>;

  function createFixture(data: { title?: string; message: string; code?: string }) {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
    TestBed.configureTestingModule({
      imports: [ErrorDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: data }
      ]
    });
    return TestBed.createComponent(ErrorDialogComponent);
  }

  it('renders the given error_code and error_message', () => {
    const fixture = createFixture({ message: 'Page 0 is invalid.', code: 'invalid_page' });
    fixture.detectChanges();

    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.error-message')?.textContent).toContain('Page 0 is invalid.');
    expect(el.querySelector('.error-code')?.textContent).toContain('invalid_page');
  });

  it('defaults the title when none is provided', () => {
    const fixture = createFixture({ message: 'Something failed.' });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h2').textContent).toContain('Error Occurred');
  });

  it('closes the dialog when Dismiss is clicked', () => {
    const fixture = createFixture({ message: 'Something failed.' });
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.dismiss-btn').click();

    expect(dialogRefSpy.close).toHaveBeenCalled();
  });
});
