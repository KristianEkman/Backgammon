import { Component, Input } from '@angular/core';
import { DiceDto } from 'src/app/dto/diceDto';
@Component({
  selector: 'app-dices',
  templateUrl: './dices.component.html',
  styleUrls: ['./dices.component.scss']
})
export class DicesComponent {
  @Input() dices: DiceDto[] | null = [];
  @Input() showRollButton = false;
}
