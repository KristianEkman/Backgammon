import { Component, Input } from '@angular/core';
import { PlayerColor } from 'src/app/dto';
import { DiceDto } from 'src/app/dto/diceDto';
@Component({
  selector: 'app-dices',
  templateUrl: './dices.component.html',
  styleUrls: ['./dices.component.scss']
})
export class DicesComponent {
  @Input() dices: DiceDto[] | null = [];
  @Input() color: PlayerColor | null = PlayerColor.neither;

  PlayerColor = PlayerColor;

  faClass = [
    'fas fa-dice-one',
    'fas fa-dice-two',
    'fas fa-dice-three',
    'fas fa-dice-four',
    'fas fa-dice-five',
    'fas fa-dice-six'
  ];
}
