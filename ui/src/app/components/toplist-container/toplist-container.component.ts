import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { Toplist } from 'src/app/dto';
import { ToplistService } from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-toplist-container',
  templateUrl: './toplist-container.component.html',
  styleUrls: ['./toplist-container.component.scss']
})
export class ToplistContainerComponent {
  @Output() bannerClick = new EventEmitter<boolean>();
  @Input() banner = false;

  toplist$: Observable<Toplist>;

  constructor(
    private topListService: ToplistService,
    private appState: AppStateService
  ) {
    this.toplist$ = this.appState.toplist.observe();
    this.topListService.loadToplist();
  }

  bannerClicked(): void {
    this.bannerClick.emit(true);
  }
}
