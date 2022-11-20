import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { format, formatISO, parseISO } from 'date-fns';
import { PlayedGameListDto, PlayerColor } from 'src/app/dto';
import { AdminService } from 'src/app/services/admin.service';

@Component({
  selector: 'app-played-games',
  templateUrl: './played-games.component.html',
  styleUrls: ['./played-games.component.scss']
})
export class PlayedGamesComponent implements OnInit {
  @Input() list: PlayedGameListDto | null = null;
  @Output() loadMore = new EventEmitter<void>();
  @Output() reload = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}

  PlayerColor = PlayerColor;

  formatDate(utcDate: Date): string {
    if (!utcDate) {
      return '';
    }

    // trick to convert to browsers local time.
    const date = new Date(utcDate.toString() + 'Z');

    return format(date, 'yyyy-MM-dd HH:mm:ss');
  }

  getColor(color: PlayerColor | undefined): string {
    if (color === undefined) {
      return '';
    }
    return PlayerColor[color];
  }

  onScroll(event: Event): void {
    const div = event.target as HTMLDivElement;
    if (div.scrollTop + div.clientHeight + 1 >= div.scrollHeight) {
      this.loadMore.emit();
    }
  }

  refresh() {
    this.reload.emit();
  }
}
