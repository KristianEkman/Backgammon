/* Auto Generated */

import { PlayerColor } from "./playerColor";
import { MoveDto } from "./moveDto";

export interface MoveDto {
    color: PlayerColor;
    from: number;
    nextMoves: MoveDto[];
    to: number;
}
