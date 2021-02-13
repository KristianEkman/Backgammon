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

  sendMoves(): void {
    console.log(this.userMoves);
  }

  addMove(move: MoveDto): void {
    this.userMoves.push({ ...move, nextMoves: [] }); // server does not need to know nextMoves.
    const prevGame = AppState.Singleton.game.getValue();
    this.gameHistory.push(prevGame);

    const clone = JSON.parse(JSON.stringify(prevGame)) as GameDto;
    clone.validMoves = move.nextMoves;
    const isWhite = prevGame.currentPlayer === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;
    const checker = <CheckerDto>clone.points[from].checkers.pop();
    clone.points[to].checkers.push(checker);
    AppState.Singleton.game.setValue(clone);
  }
}
