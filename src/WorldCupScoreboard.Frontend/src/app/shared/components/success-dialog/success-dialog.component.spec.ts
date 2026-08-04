import { TestBed } from '@angular/core/testing';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SuccessDialogComponent } from './success-dialog.component';

describe('SuccessDialogComponent', () => {
  let dialogRefSpy: jasmine.SpyObj<MatDialogRef<SuccessDialogComponent>>;

  function createFixture(data: { title?: string; message: string }) {
    dialogRefSpy = jasmine.createSpyObj('MatDialogRef', ['close']);
    TestBed.configureTestingModule({
      imports: [SuccessDialogComponent],
      providers: [
        { provide: MatDialogRef, useValue: dialogRefSpy },
        { provide: MAT_DIALOG_DATA, useValue: data }
      ]
    });
    return TestBed.createComponent(SuccessDialogComponent);
  }

  it('renders the given message', () => {
    const fixture = createFixture({ message: 'Match started successfully.', title: 'Match Started' });
    fixture.detectChanges();

    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.success-message')?.textContent).toContain('Match started successfully.');
    expect(el.querySelector('h2')?.textContent).toContain('Match Started');
  });

  it('defaults the title to "Success" when none is provided', () => {
    const fixture = createFixture({ message: 'Done.' });
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('h2').textContent).toContain('Success');
  });

  it('closes the dialog when OK is clicked', () => {
    const fixture = createFixture({ message: 'Done.' });
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.ok-btn').click();

    expect(dialogRefSpy.close).toHaveBeenCalled();
  });
});
