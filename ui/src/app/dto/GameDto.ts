import { DiceDto, MoveDto, PlayerColor, PlayerDto, PointDto } from '.';

export interface GameDto {
  BlackPlayer: PlayerDto;
  WhitePlayer: PlayerDto;

  CurrentPlayer: PlayerColor;
  PlayState: GameState;
  Points: PointDto[];
  Roll: DiceDto[];
  ValidMoves: MoveDto[];
}

export enum GameState {
  FirstThrow,
  Playing,
  Ended
}
