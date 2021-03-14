/* Auto Generated */

import { ActionDto } from "./actionDto";
import { GameDto } from "./../gameDto";
import { NewScoreDto } from "./../toplist/newScoreDto";

export interface GameEndedActionDto extends ActionDto {
    game: GameDto;
    newScore: NewScoreDto;
}
