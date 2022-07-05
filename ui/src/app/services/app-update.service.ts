import { Injectable } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class AppUpdateService {
  constructor(private updates: SwUpdate, private appState: AppStateService) {
    updates.versionUpdates.subscribe((evt) => {
      switch (evt.type) {
        case 'VERSION_DETECTED':
          console.log(`Downloading new app version: ${evt.version.hash}`);
          break;
        case 'VERSION_READY':
          console.log(`Current app version: ${evt.currentVersion.hash}`);
          console.log(
            `New app version ready for use: ${evt.latestVersion.hash}`
          );
          this.updates.activateUpdate().then(() => {
            setTimeout(() => {
              this.appState.newVersion.setValue(true);
            }, 1);
          });
          break;
        case 'VERSION_INSTALLATION_FAILED':
          console.log(
            `Failed to install app version '${evt.version.hash}': ${evt.error}`
          );
          break;
      }
    });
  }

  update() {
    document.location.reload();
  }
}
