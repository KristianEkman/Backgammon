import { Component, EventEmitter, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-select-language',
  templateUrl: './select-language.component.html',
  styleUrls: ['./select-language.component.scss']
})
export class SelectLanguageComponent {
  Language = Language;

  @Output() changed = new EventEmitter<void>();
  constructor(public translateService: TranslateService) {}

  useLang(lang: string): void {
    this.translateService.use(lang);
    this.changed.emit();
  }
}

export class Language {
  constructor(public code: string, public name: string) {}
  static get List(): Language[] {
    return [
      new Language('en', 'English'),
      new Language('zh', '中國人'),
      new Language('es', 'Español'),
      new Language('ar', 'عربى'),
      new Language('fr', 'français'),
      new Language('sv', 'Svenska')
      // new Language('x', 'X')
    ];
  }
}
