import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CheckerDto,
  DiceDto,
  GameDto,
  MoveDto,
  PlayerColor
} from 'src/app/dto';
import { SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent {
  constructor(private socket: SocketsService) {
    this.gameDto$ = AppState.Singleton.game.changed;
    this.dices$ = AppState.Singleton.dices.changed;
    socket.connect();
  }

  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;

  userMoves: MoveDto[] = [];
  gameHistory: GameDto[] = [];
  dicesHistory: DiceDto[][] = [];

  sendMoves(): void {
    console.log(this.userMoves);
  }

  addMove(move: MoveDto): void {
    this.userMoves.push({ ...move, nextMoves: [] }); // server does not need to know nextMoves.
    const prevGame = AppState.Singleton.game.getValue();
    this.gameHistory.push(prevGame);

    const gameClone = JSON.parse(JSON.stringify(prevGame)) as GameDto;
    gameClone.validMoves = move.nextMoves;
    const isWhite = prevGame.currentPlayer === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;
    const checker = <CheckerDto>gameClone.points[from].checkers.pop();
    gameClone.points[to].checkers.push(checker);
    AppState.Singleton.game.setValue(gameClone);

    const dices = AppState.Singleton.dices.getValue();
    this.dicesHistory.push(dices);

    const diceClone = JSON.parse(JSON.stringify(dices)) as DiceDto[];
    const diceIdx = diceClone.findIndex(
      (d) => !d.used && d.value === move.to - move.from
    );
    diceClone[diceIdx].used = true;
    AppState.Singleton.dices.setValue(diceClone);
  }

  undoMove(): void {
    if (this.gameHistory.length < 1) {
      return;
    }
    const game = this.gameHistory.pop() as GameDto;
    AppState.Singleton.game.setValue(game);

    const dices = this.dicesHistory.pop() as DiceDto[];
    AppState.Singleton.dices.setValue(dices);
  }
}
