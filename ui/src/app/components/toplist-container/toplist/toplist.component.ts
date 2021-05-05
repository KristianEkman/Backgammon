import { Component, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Toplist, ToplistResult } from 'src/app/dto';

@Component({
  selector: 'app-toplist',
  templateUrl: './toplist.component.html',
  styleUrls: ['./toplist.component.scss']
})
export class ToplistComponent {
  @Input() toplist: Toplist | null = null;
  constructor(private trans: TranslateService) {}

  flipped = false;

  getName(item: ToplistResult): string {
    const you = this.trans.instant('toplist.you');
    return item.you ? `${item.name} (${you})` : item.name;
  }
}
