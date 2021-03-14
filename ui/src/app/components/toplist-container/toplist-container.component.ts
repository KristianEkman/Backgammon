import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Toplist } from 'src/app/dto';

@Component({
  selector: 'app-toplist-container',
  templateUrl: './toplist-container.component.html',
  styleUrls: ['./toplist-container.component.scss']
})
export class ToplistContainerComponent {
  @Input() toplist: Toplist | null = null;

  @Output() showing = new EventEmitter<boolean>();
  banner = true;
  constructor() {}

  bannerClicked(): void {
    this.banner = false;
    this.showing.emit(true);
  }

  topListClosed(): void {
    this.banner = true;
    this.showing.emit(false);
  }
}
