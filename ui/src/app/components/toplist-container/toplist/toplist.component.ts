import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Toplist, ToplistResult } from 'src/app/dto';

@Component({
  selector: 'app-toplist',
  templateUrl: './toplist.component.html',
  styleUrls: ['./toplist.component.scss']
})
export class ToplistComponent implements OnInit {
  @Input() toplist: Toplist | null = null;
  @Output() closed = new EventEmitter<void>();
  constructor(private trans: TranslateService) {}

  ngOnInit(): void {}

  closeClick(): void {
    this.closed.emit();
  }

  getName(item: ToplistResult): string {
    const you = this.trans.instant('toplist.you');
    return item.you ? `${item.name} (${you})` : item.name;
  }
}
