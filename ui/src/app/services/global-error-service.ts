import { ErrorHandler, Injectable, NgZone } from '@angular/core';
import { AppStateService } from '../state/app-state.service';
import { ErrorState } from '../state/ErrorState';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorService implements ErrorHandler {
  constructor(private zone: NgZone, private appState: AppStateService) {}

  handleError(error: any): void {
    if (!error) {
      return;
    }
    console.error(error);
    let current = this.appState.errors.getValue()?.message ?? '';
    let sError = error.stack ?? '';
    sError += error.message ?? error;

    // This is actually no error I suppose.
    if (sError.indexOf('popup_closed_by_user') > -1) {
      return;
    }

    if (sError.indexOf('Not logged in') > -1) {
      return;
    }

    if (sError.indexOf('ExpressionChangedAfterItHasBeenCheckedError') > -1) {
      return;
    }

    const date = new Date();
    const err = date + '\n' + sError + '\n\n';
    current += err;
    this.zone.run(() => {
      this.appState.errors.setValue(new ErrorState(current));
    });
  }
}
