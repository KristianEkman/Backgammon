import { Component, Input, OnChanges } from '@angular/core';
import { Language } from 'src/app/utils';

@Component({
  selector: 'app-language-hint',
  templateUrl: './language-hint.component.html',
  styleUrls: ['./language-hint.component.scss']
})
export class LanguageHintComponent implements OnChanges {
  @Input() acceptedLangs: string[] | null = [];
  @Input() selectedLang: string = '';

  languageName = 'English';
  languageCode = 'en';
  show = false;

  ngOnChanges(): void {
    var accLangs = this.acceptedLangs;
    if (!accLangs || !this.selectedLang) return;

    this.show = false;
    for (let i = 0; i < Language.List.length; i++) {
      for (let j = 0; j < accLangs.length; j++) {
        const a = Language.List[i];
        const b = accLangs[j];
        if (a.code === b) {
          if (b !== this.selectedLang) {
            this.languageName = a.name;
            this.languageCode = a.code;
            this.show = true;
            break;
          }
        }
      }
      if (this.show) break;
    }
  }
}
