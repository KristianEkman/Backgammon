import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CheckerDto,
  DiceDto,
  GameDto,
  MoveDto,
  PlayerColor
} from '../dto';
import {
  ActionDto,
  ActionNames,
  DicesRolledActionDto,
  GameCreatedActionDto,
  GameEndedActionDto,
  MovesMadeActionDto
} from '../dto/Actions';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class SocketsService {
  socket: WebSocket | undefined;
  url = '';
  userMoves: MoveDto[] = [];
  gameHistory: GameDto[] = [];
  dicesHistory: DiceDto[][] = [];

  constructor() {
    // this.socket = new WebSocket('ws://localhost:60109/ws');
  }

  connect(): void {
    this.url = environment.socketServiceUrl;
    this.socket = new WebSocket(this.url);
    this.socket.onmessage = this.onMessage.bind(this);
    this.socket.onerror = this.onError.bind(this);
    this.socket.onopen = this.onOpen.bind(this);
    this.socket.onclose = this.onClose.bind(this);
  }

  onOpen(event: Event): void {
    console.log('Open', { event });
  }

  doOpponentMove(move: MoveDto): void {
    const game = AppState.Singleton.game.getValue();
    const gameClone = JSON.parse(JSON.stringify(game)) as GameDto;
    const isWhite = move.color === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;
    const checker = <CheckerDto>gameClone.points[from].checkers.pop();

    // hitting opponent checker
    const hit = gameClone.points[to].checkers.find(
      (c) => c.color !== move.color
    );
    if (hit) {
      gameClone.points[to].checkers.pop();
      const barIdx = isWhite ? 0 : 25;
      gameClone.points[barIdx].checkers.push(hit);
    }

    gameClone.points[to].checkers.push(checker);

    AppState.Singleton.game.setValue(gameClone);
  }

  doMove(move: MoveDto): void {
    this.userMoves.push({ ...move, nextMoves: [] }); // server does not need to know nextMoves.
    const prevGame = AppState.Singleton.game.getValue();
    this.gameHistory.push(prevGame);

    const gameClone = JSON.parse(JSON.stringify(prevGame)) as GameDto;
    gameClone.validMoves = move.nextMoves;
    const isWhite = move.color === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;
    // remove moved checker
    const checker = <CheckerDto>gameClone.points[from].checkers.pop();

    // hitting opponent checker
    const hit = gameClone.points[to].checkers.find(
      (c) => c.color !== move.color
    );
    if (hit) {
      gameClone.points[to].checkers.pop();
      const barIdx = isWhite ? 0 : 25;
      gameClone.points[barIdx].checkers.push(hit);
    }

    //push checker to new point
    gameClone.points[to].checkers.push(checker);
    AppState.Singleton.game.setValue(gameClone);

    const dices = AppState.Singleton.dices.getValue();
    this.dicesHistory.push(dices);

    const diceClone = JSON.parse(JSON.stringify(dices)) as DiceDto[];

    // Find a dice with equal value as the move length
    // or if bearing off equal or larger
    let diceIdx = diceClone.findIndex(
      (d) => !d.used && d.value === move.to - move.from
    );

    if (diceIdx < 0) {
      diceIdx = diceClone.findIndex(
        (d) => move.to === 25 && move.to - move.from <= d.value
      );
    }
    const dice = diceClone[diceIdx];
    dice.used = true;
    AppState.Singleton.dices.setValue(diceClone);
  }

  undoMove(): void {
    if (this.gameHistory.length < 1) {
      return;
    }
    this.userMoves.pop();
    const game = this.gameHistory.pop() as GameDto;
    AppState.Singleton.game.setValue(game);

    const dices = this.dicesHistory.pop() as DiceDto[];
    AppState.Singleton.dices.setValue(dices);
  }

  onMessage(message: MessageEvent<string>): void {
    const action = JSON.parse(message.data) as ActionDto;
    // console.log(message.data);
    const game = AppState.Singleton.game.getValue();
    switch (action.actionName) {
      case ActionNames.gameCreated:
        const dto = JSON.parse(message.data) as GameCreatedActionDto;
        AppState.Singleton.myColor.setValue(dto.myColor);
        AppState.Singleton.game.setValue(dto.game);
        break;
      case ActionNames.dicesRolled:
        const dicesAction = JSON.parse(message.data) as DicesRolledActionDto;
        AppState.Singleton.dices.setValue(dicesAction.dices);
        const cGame = {
          ...game,
          validMoves: dicesAction.validMoves,
          currentPlayer: dicesAction.playerToMove
        };
        AppState.Singleton.game.setValue(cGame);
        break;
      case ActionNames.movesMade:
        const movesAction = JSON.parse(message.data) as MovesMadeActionDto;
        for (let i = 0; i < movesAction.moves.length; i++) {
          const move = movesAction.moves[i];
          this.doOpponentMove(move);
        }
        break;
      case ActionNames.gameEnded:
        const endedAction = JSON.parse(message.data) as GameEndedActionDto;
        console.log('game ended', endedAction.game.winner);

        AppState.Singleton.game.setValue(endedAction.game);
        break;
      default:
        throw new Error(`Action not implemented ${action.actionName}`);
    }
  }

  onError(event: Event): void {
    console.error('Error', { event });
  }

  sendMessage(message: string): void {
    if (this.socket) {
      this.socket.send(message);
    }
  }

  onClose(event: CloseEvent): void {
    console.log('Close', { event });
  }

  sendMoves(): void {
    const action: MovesMadeActionDto = {
      actionName: ActionNames.movesMade,
      moves: this.userMoves
    };
    this.sendMessage(JSON.stringify(action));
    this.userMoves = [];
    this.dicesHistory = [];
    this.gameHistory = [];
  }
}
