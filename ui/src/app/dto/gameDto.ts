/* Auto Generated */

import { PlayerColor } from './playerColor';
import { PlayerDto } from './playerDto';
import { GameState } from './gameState';
import { PointDto } from './pointDto';
import { MoveDto } from './moveDto';

export interface GameDto {
  id: string;
  myColor: PlayerColor;
  blackPlayer: PlayerDto;
  whitePlayer: PlayerDto;
  currentPlayer: PlayerColor;
  playState: GameState;
  points: PointDto[];
  validMoves: MoveDto[];
}
