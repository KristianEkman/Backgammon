/* Auto Generated */

import { PlayerDto } from './playerDto';
import { PlayerColor } from './playerColor';
import { GameState } from './gameState';
import { PointDto } from './pointDto';
import { DiceDto } from './diceDto';
import { MoveDto } from './moveDto';

export interface GameDto {
  id: string;
  blackPlayer: PlayerDto;
  whitePlayer: PlayerDto;
  currentPlayer: PlayerColor;
  playState: GameState;
  points: PointDto[];
  roll: DiceDto[];
  validMoves: MoveDto[];
}
