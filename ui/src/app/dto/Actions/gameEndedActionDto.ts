/* Auto Generated */

import { ActionDto } from './actionDto';
import { GameDto } from './../gameDto';
import { NewScoreDto } from '..';

export interface GameEndedActionDto extends ActionDto {
  game: GameDto;
  newScore: NewScoreDto;
}
