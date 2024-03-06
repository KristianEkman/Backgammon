import { Component, Input } from '@angular/core';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { Toplist, ToplistResult } from 'src/app/dto';
import { ButtonComponent } from '../../shared/button/button.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-toplist',
  standalone: true,
  templateUrl: './toplist.component.html',
  styleUrls: ['./toplist.component.scss'],
  imports: [ButtonComponent, CommonModule, TranslateModule]
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
