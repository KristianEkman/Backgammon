import { Component, OnInit } from '@angular/core';
import { AppState } from './state/app-state';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon';
  busy$ = AppState.Singleton.busy.observe();
}
