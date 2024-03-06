import { Injectable } from '@angular/core';
import { AppStateService } from 'src/app/state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class Theme {
  constructor(private appState: AppStateService) {}
}
