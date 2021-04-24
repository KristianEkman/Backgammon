import { Component } from '@angular/core';
import { AppState } from './state/app-state';
import { AccountService, ErrorReportService } from './services';
import { Observable } from 'rxjs';
import { ErrorState } from './state/ErrorState';
import { ErrorReportDto } from './dto';
import { TranslateService } from '@ngx-translate/core';
import { Language } from './components/select-language/select-language.component';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon';
  busy$ = AppState.Singleton.busy.observe();
  errors$: Observable<ErrorState>;

  constructor(
    private accountService: AccountService,
    private errorReportService: ErrorReportService,
    private translateService: TranslateService
  ) {
    this.errors$ = AppState.Singleton.errors.observe();
    this.accountService.repair();

    const langs = Language.List.map((l) => l.code);
    this.translateService.addLangs(langs);
    this.translateService.setDefaultLang('en');
    const browserLang = this.translateService.getBrowserLang();
    let startLang = 'en';

    for (let i = 0; i < langs.length; i++) {
      if (browserLang.indexOf(langs[i]) > -1) {
        startLang = langs[i];
      }
    }

    this.translateService.use(startLang);
  }

  saveErrorReport(errorDto: ErrorReportDto): void {
    this.errorReportService.saveErrorReport(errorDto);
  }
}
