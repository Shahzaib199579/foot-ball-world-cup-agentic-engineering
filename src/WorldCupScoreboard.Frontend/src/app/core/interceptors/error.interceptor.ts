import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ErrorDialogService } from '../services/error-dialog.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorDialogService = inject(ErrorDialogService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'An unexpected error occurred. Please try again.';
      let errorCode = undefined;

      if (error.error) {
        if (typeof error.error === 'object') {
          if (error.error.error_message) {
            errorMessage = error.error.error_message;
          }
          if (error.error.error_code) {
            errorCode = error.error.error_code;
          }
        } else if (typeof error.error === 'string') {
          errorMessage = error.error;
        }
      }

      if (error.status === 0) {
        errorMessage = 'Unable to connect to the backend server. Please make sure the service is running.';
      }

      errorDialogService.openError(errorMessage, 'Request Error', errorCode);
      return throwError(() => error);
    })
  );
};
