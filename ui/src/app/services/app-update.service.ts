import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class AppUpdateService {
  constructor(private updates: SwUpdate) {
    updates.available.subscribe((event) => {
      console.log('current version is', event.current);
      console.log('available version is', event.available);
      this.updates.activateUpdate().then(() => {
        setTimeout(() => {
          AppState.Singleton.newVersion.setValue(true);
        }, 1);
      });
    });

    this.updates.activated.subscribe((event) => {
      console.log('old version was', event.previous);
      console.log('new version is', event.current);
    });
  }

  update() {
    this.updates.activateUpdate().then(() => {
      AppState.Singleton.newVersion.setValue(false);
      document.location.reload();
    });
  }
}
