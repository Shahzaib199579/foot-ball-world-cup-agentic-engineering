import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ErrorDialogService } from '../services/error-dialog.service';
import { errorInterceptor } from './error.interceptor';

describe('errorInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;
  let errorDialogSpy: jasmine.SpyObj<ErrorDialogService>;

  beforeEach(() => {
    errorDialogSpy = jasmine.createSpyObj('ErrorDialogService', ['openError']);
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([errorInterceptor])),
        provideHttpClientTesting(),
        { provide: ErrorDialogService, useValue: errorDialogSpy }
      ]
    });
    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('opens the error dialog with error_code/error_message from a JSON error body', () => {
    http.get('/matches/999').subscribe({ error: () => undefined });

    httpMock.expectOne('/matches/999').flush(
      { error_code: 'match_not_found', error_message: 'No in-progress match was found with Id 999.' },
      { status: 404, statusText: 'Not Found' }
    );

    expect(errorDialogSpy.openError).toHaveBeenCalledWith(
      'No in-progress match was found with Id 999.',
      'Request Error',
      'match_not_found'
    );
  });

  it('shows a connection-failure message when the request cannot reach the server', () => {
    http.get('/matches/summary').subscribe({ error: () => undefined });

    httpMock.expectOne('/matches/summary').flush(null, { status: 0, statusText: 'Unknown Error' });

    expect(errorDialogSpy.openError).toHaveBeenCalledWith(
      'Unable to connect to the backend server. Please make sure the service is running.',
      'Request Error',
      undefined
    );
  });

  it('re-throws the error so callers can still react to it', () => {
    let caught: unknown;
    http.get('/matches/999').subscribe({ error: (err) => (caught = err) });

    httpMock.expectOne('/matches/999').flush(
      { error_code: 'match_not_found', error_message: 'not found' },
      { status: 404, statusText: 'Not Found' }
    );

    expect(caught).toBeTruthy();
  });
});
