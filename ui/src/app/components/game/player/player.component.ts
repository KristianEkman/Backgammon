import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { PlayerDto } from 'src/app/dto';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnChanges {
  constructor() {}

  @Input() playerDto?: PlayerDto;
  @Input() doubling: number | null = null;

  lokalGold = 0;

  ngOnChanges(): void {
    if (this.playerDto && this.playerDto.gold !== this.lokalGold) {
      const gold = this.playerDto.gold;
      var step = this.playerDto.gold > this.lokalGold ? 1 : -1;
      const handle = setInterval(() => {
        this.lokalGold += step;
        if (
          (step > 0 && this.lokalGold >= gold) ||
          (step < 0 && this.lokalGold <= gold)
        ) {
          this.lokalGold = gold;
          clearInterval(handle);
          return;
        }
      }, 10);
    }
  }
}
