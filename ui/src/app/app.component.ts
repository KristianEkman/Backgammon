import { Component, HostListener, Inject } from '@angular/core';
import { AppStateService } from './state/app-state.service';
import { AccountService, ErrorReportService } from './services';
import { Observable } from 'rxjs';
import { ErrorState } from './state/ErrorState';
import { ErrorReportDto } from './dto';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { CommonModule, DOCUMENT } from '@angular/common';
import {
  NavigationEnd,
  NavigationStart,
  Router,
  RouterModule
} from '@angular/router';
import { Language } from './utils';
import { Busy } from './state/busy';
import { HomeButtonComponent } from './components/shared/home-button/home-button.component';
import { BusyComponent } from './components/shared/busy/busy.component';
import { ErrorHandlerComponent } from './components/shared/error-handler/error-handler.component';

@Component({
  selector: 'app-root',
  standalone: true,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  imports: [
    CommonModule,
    HomeButtonComponent,
    BusyComponent,
    ErrorHandlerComponent,
    RouterModule
  ]
})
export class AppComponent {
  title = 'Backgammon';
  hide = false;
  busy$: Observable<Busy>;
  errors$: Observable<ErrorState>;

  constructor(
    private accountService: AccountService,
    private errorReportService: ErrorReportService,
    private translateService: TranslateService,
    @Inject(DOCUMENT) private document: Document,
    private router: Router,
    private appState: AppStateService
  ) {
    this.errors$ = this.appState.errors.observe();
    this.busy$ = this.appState.busy.observe();
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

  @HostListener('document:keydown.escape', ['$event'])
  onKeydownHandler() {
    this.appState.chatOpen.setValue(false);
  }

  saveErrorReport(errorDto: ErrorReportDto): void {
    this.errorReportService.saveErrorReport(errorDto);
  }
}
