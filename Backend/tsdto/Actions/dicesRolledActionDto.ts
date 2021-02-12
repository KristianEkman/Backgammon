/* Auto Generated */

import { ActionDto } from "./actionDto";
import { DiceDto } from "./../diceDto";
import { PlayerColor } from "./../playerColor";
import { MoveDto } from "./../moveDto";

export interface DicesRolledActionDto extends ActionDto {
    dices: DiceDto[];
    playerToMove: PlayerColor;
    validMoves: MoveDto[];
}
