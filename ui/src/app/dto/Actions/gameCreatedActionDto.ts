/* Auto Generated */

import { ActionDto } from './actionDto';
import { GameDto } from './../gameDto';
import { PlayerColor } from './../playerColor';

export interface GameCreatedActionDto extends ActionDto {
  game: GameDto;
  myColor: PlayerColor;
}
