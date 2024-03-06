import { Component, EventEmitter, Input, Output } from '@angular/core';
import { format } from 'date-fns';
import { PlayedGameListDto, PlayerColor } from 'src/app/dto';
import { ButtonComponent } from '../../shared/button/button.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-played-games',
  standalone: true,
  templateUrl: './played-games.component.html',
  styleUrls: ['./played-games.component.scss'],
  imports: [ButtonComponent, CommonModule]
})
export class PlayedGamesComponent {
  @Input() list: PlayedGameListDto | null = null;
  @Output() loadMore = new EventEmitter<void>();
  @Output() reload = new EventEmitter<void>();

  constructor() {}

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
