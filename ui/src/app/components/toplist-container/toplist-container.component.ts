import { Component, EventEmitter, Output } from '@angular/core';
import { Observable } from 'rxjs';
import { Toplist } from 'src/app/dto';
import { ToplistService } from 'src/app/services';

@Component({
  selector: 'app-toplist-container',
  templateUrl: './toplist-container.component.html',
  styleUrls: ['./toplist-container.component.scss']
})
export class ToplistContainerComponent {
  toplist$: Observable<Toplist>;

  @Output() showing = new EventEmitter<boolean>();
  banner = true;
  constructor(private service: ToplistService) {
    this.toplist$ = this.service.getToplist();
  }

  bannerClicked(): void {
    this.banner = false;
    this.showing.emit(true);
  }

  topListClosed(): void {
    this.banner = true;
    this.showing.emit(false);
  }
}
