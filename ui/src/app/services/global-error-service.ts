/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
import { ErrorHandler, Injectable, NgZone } from '@angular/core';
import { AppState } from '../state/app-state';
import { ErrorState } from '../state/ErrorState';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorService implements ErrorHandler {
  constructor(private zone: NgZone) {}

  handleError(error: any): void {
    if (!error) {
      return;
    }
    let current = AppState.Singleton.errors.getValue()?.message ?? '';

    let sError = error.stack ?? '';
    sError += error.message ?? error;
    const date = new Date();
    const err = date + '\n' + sError + '\n\n';
    current += err;
    this.zone.run(() => {
      AppState.Singleton.errors.setValue(new ErrorState(current));
    });
  }
}
