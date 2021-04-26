import { Component, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Busy } from 'src/app/state/busy';

@Component({
  selector: 'app-busy',
  templateUrl: './busy.component.html',
  styleUrls: ['./busy.component.scss']
})
export class BusyComponent {
  constructor(private trans: TranslateService) {
    trans.onLangChange.subscribe(() => {
      this.text = trans.instant('pleasewait');
    });
  }

  @Input() busy: Busy | null = null;
  @Input() overlay = true;
  text = '';
}
