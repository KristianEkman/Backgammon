import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { format, formatISO, parseISO } from 'date-fns';
import { PlayedGameListDto, PlayerColor } from 'src/app/dto';

@Component({
  selector: 'app-played-games',
  templateUrl: './played-games.component.html',
  styleUrls: ['./played-games.component.scss']
})
export class PlayedGamesComponent implements OnInit {
  @Input() list: PlayedGameListDto | null = null;
  @Output() loadAfter = new EventEmitter<string>();

  constructor() {}

  ngOnInit(): void {}

  PlayerColor = PlayerColor;

  formatDate(sDate: any): string {
    if (!sDate) {
      return '';
    }
    const date = new Date(Date.parse(sDate));

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
      const lastDate = this.list?.games[this.list?.games.length - 1].started;

      if (!lastDate) {
        return;
      }

      const parsed = parseISO(lastDate.toString());
      const s = format(parsed, 'yyyy-MM-dd HH:mm:ss.SSS');

      this.loadAfter.emit(s);
    }
  }
}
