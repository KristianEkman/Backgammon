/* Auto Generated */

import { PlayerColor } from './playerColor';

export interface MoveDto {
  color: PlayerColor;
  from: number;
  nextMoves: MoveDto[];
  to: number;
  animate: boolean;
}
