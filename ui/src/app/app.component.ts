import { Component } from '@angular/core';
import { AppState } from './state/app-state';
import { AccountService } from './services';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon';
  busy$ = AppState.Singleton.busy.observe();

  constructor(private accountService: AccountService) {
    this.accountService.repair();
  }
}
