import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { AppState } from './state/app-state';
import { AccountService, ErrorReportService } from './services';
import { Observable } from 'rxjs';
import { ErrorState } from './state/ErrorState';
import { ErrorReportDto } from './dto';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { Language } from './components/select-language/select-language.component';
import { DOCUMENT } from '@angular/common';
import { NavigationEnd, NavigationStart, Router } from '@angular/router';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Backgammon';
  hide = false;
  busy$ = AppState.Singleton.busy.observe();
  errors$: Observable<ErrorState>;

  constructor(
    private accountService: AccountService,
    private errorReportService: ErrorReportService,
    private translateService: TranslateService,
    @Inject(DOCUMENT) private document: Document,
    private router: Router
  ) {
    this.errors$ = AppState.Singleton.errors.observe();
    this.accountService.repair();

    const langs = Language.List.map((l) => l.code);
    this.translateService.addLangs(langs);
    this.translateService.setDefaultLang('en');
    // const browserLang = this.translateService.getBrowserLang();
    // let startLang = 'en';

    // for (let i = 0; i < langs.length; i++) {
    //   if (browserLang.indexOf(langs[i]) > -1) {
    //     startLang = langs[i];
    //   }
    // }

    this.translateService.onLangChange.subscribe((event: LangChangeEvent) => {
      // this is for helping screen readers and external translation tools
      this.document.documentElement.lang = event.lang;
    });

    // this.translateService.use(startLang);

    this.router.events.subscribe((event) => {
      if (event instanceof NavigationStart) {
        this.hide = true;
      }

      if (event instanceof NavigationEnd) {
        setTimeout(() => {
          this.hide = false;
        }, 1);
      }
    });
  }

  saveErrorReport(errorDto: ErrorReportDto): void {
    this.errorReportService.saveErrorReport(errorDto);
  }
}
