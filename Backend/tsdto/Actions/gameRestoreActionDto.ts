/* Auto Generated */

import { ActionDto } from "./actionDto";
import { GameDto } from "./../gameDto";
import { PlayerColor } from "./../playerColor";
import { DiceDto } from "./../diceDto";

export interface GameRestoreActionDto extends ActionDto {
    game: GameDto;
    color: PlayerColor;
    dices: DiceDto[];
}
