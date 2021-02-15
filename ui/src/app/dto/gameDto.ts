/* Auto Generated */

import { PlayerDto } from './playerDto';
import { PlayerColor } from './playerColor';
import { GameState } from './gameState';
import { PointDto } from './pointDto';
import { MoveDto } from './moveDto';

export interface GameDto {
  id: string;
  blackPlayer: PlayerDto;
  whitePlayer: PlayerDto;
  currentPlayer: PlayerColor;
  winner: PlayerColor;
  playState: GameState;
  points: PointDto[];
  validMoves: MoveDto[];
}
