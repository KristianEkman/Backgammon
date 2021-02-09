import { PlayerColor } from '.';

export interface MoveDto {
  Color: PlayerColor;
  From: number;
  NextMoves: MoveDto[];
  To: number;
}
