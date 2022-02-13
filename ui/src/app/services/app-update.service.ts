import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class AppUpdateService {
  constructor(private updates: SwUpdate) {
    updates.versionUpdates.subscribe((event) => {
      // console.log('current version is', event);
      // console.log('available version is', event);
      this.updates.activateUpdate().then(() => {
        setTimeout(() => {
          AppState.Singleton.newVersion.setValue(true);
        }, 1);
      });
    });

    // this.updates.activateUpdate().then((event) => {
    //   console.log('old version was', event);
    //   console.log('new version is', event);
    // });
  }

  update() {
    document.location.reload();
  }
}
