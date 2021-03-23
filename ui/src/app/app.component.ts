import { Component } from '@angular/core';
import { AppState } from './state/app-state';
import { AccountService } from './services';
import { Observable } from 'rxjs';
import { ErrorState } from './state/ErrorState';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon';
  busy$ = AppState.Singleton.busy.observe();
  errors$: Observable<ErrorState>;

  constructor(private accountService: AccountService) {
    this.errors$ = AppState.Singleton.errors.observe();

    this.accountService.repair();
  }
}
