import { Injectable, OnDestroy } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CheckerDto,
  DiceDto,
  GameDto,
  MoveDto,
  PlayerColor,
  GameCookieDto
} from '../dto';
import { CookieService } from 'ngx-cookie-service';
import {
  ActionDto,
  ActionNames,
  DicesRolledActionDto,
  GameCreatedActionDto,
  GameEndedActionDto,
  MovesMadeActionDto,
  OpponentMoveActionDto,
  UndoActionDto,
  ConnectionInfoActionDto,
  GameRestoreActionDto,
  AbortGameActionDto
} from '../dto/Actions';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class SocketsService implements OnDestroy {
  socket: WebSocket | undefined;
  url = '';
  userMoves: MoveDto[] = [];
  gameHistory: GameDto[] = [];
  dicesHistory: DiceDto[][] = [];
  connectTime = new Date();
  constructor(private cookieService: CookieService) {}

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
    const now = new Date();
    const ping = now.getTime() - this.connectTime.getTime();
    AppState.Singleton.myConnection.setValue({ connected: true, pingMs: ping });
  }

  onError(event: Event): void {
    console.error('Error', { event });
    const cnn = AppState.Singleton.myConnection.getValue();
    AppState.Singleton.myConnection.setValue({ ...cnn, connected: false });
  }

  onClose(event: CloseEvent): void {
    console.log('Close', { event });
    const cnn = AppState.Singleton.myConnection.getValue();
    AppState.Singleton.myConnection.setValue({ ...cnn, connected: false });
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
    if (move.animate) {
      const clone = [...AppState.Singleton.moveAnimations.getValue()];
      // console.log('pushing next animation');
      clone.push(move);
      AppState.Singleton.moveAnimations.setValue(clone);
    }
  }

  undoMove(): void {
    if (this.gameHistory.length < 1) {
      return;
    }
    const move = this.userMoves.pop();
    if (!move) {
      return;
    }
    const game = this.gameHistory.pop() as GameDto;
    AppState.Singleton.game.setValue(game);

    const dices = this.dicesHistory.pop() as DiceDto[];
    AppState.Singleton.dices.setValue(dices);

    const clone = [...AppState.Singleton.moveAnimations.getValue()];
    // console.log('pushing next animation');
    clone.push({ ...move, from: move.to, to: move.from });
    AppState.Singleton.moveAnimations.setValue(clone);
  }

  onMessage(message: MessageEvent<string>): void {
    const action = JSON.parse(message.data) as ActionDto;
    // console.log(message.data);
    const game = AppState.Singleton.game.getValue();
    switch (action.actionName) {
      case ActionNames.gameCreated: {
        const dto = JSON.parse(message.data) as GameCreatedActionDto;
        AppState.Singleton.myColor.setValue(dto.myColor);
        AppState.Singleton.game.setValue(dto.game);

        const cookie: GameCookieDto = { id: dto.game.id, color: dto.myColor };
        this.cookieService.deleteAll('backgammon-game-id');
        console.log('Settings cookie', cookie);
        this.cookieService.set('backgammon-game-id', JSON.stringify(cookie), 2);
        break;
      }
      case ActionNames.dicesRolled: {
        const dicesAction = JSON.parse(message.data) as DicesRolledActionDto;
        AppState.Singleton.dices.setValue(dicesAction.dices);
        const cGame = {
          ...game,
          validMoves: dicesAction.validMoves,
          currentPlayer: dicesAction.playerToMove
        };
        // console.log(dicesAction.validMoves);
        AppState.Singleton.game.setValue(cGame);
        break;
      }
      case ActionNames.movesMade: {
        // const movesAction = JSON.parse(message.data) as MovesMadeActionDto;
        // for (let i = 0; i < movesAction.moves.length; i++) {
        //   const move = movesAction.moves[i];
        //   this.doOpponentMove(move);
        // }
        break;
      }
      case ActionNames.gameEnded: {
        const endedAction = JSON.parse(message.data) as GameEndedActionDto;
        // console.log('game ended', endedAction.game.winner);
        AppState.Singleton.game.setValue(endedAction.game);
        break;
      }
      case ActionNames.opponentMove: {
        const action = JSON.parse(message.data) as OpponentMoveActionDto;
        this.doMove(action.move);
        break;
      }
      case ActionNames.undoMove: {
        // const action = JSON.parse(message.data) as UndoActionDto;
        this.undoMove();
        break;
      }
      case ActionNames.connectionInfo: {
        const action = JSON.parse(message.data) as ConnectionInfoActionDto;
        if (!action.connection.connected) {
          console.log('Opponent disconnected');
        }
        const cnn = AppState.Singleton.opponentConnection.getValue();
        AppState.Singleton.opponentConnection.setValue({
          ...cnn,
          connected: action.connection.connected
        });
        break;
      }
      case ActionNames.gameRestore: {
        const dto = JSON.parse(message.data) as GameRestoreActionDto;
        AppState.Singleton.myColor.setValue(dto.color);
        AppState.Singleton.game.setValue(dto.game);
        AppState.Singleton.dices.setValue(dto.dices);
        break;
      }

      default:
        throw new Error(`Action not implemented ${action.actionName}`);
    }
  }

  sendMessage(message: string): void {
    if (this.socket) {
      this.socket.send(message);
    }
  }

  sendMoves(): void {
    const myColor = AppState.Singleton.myColor.getValue();
    // Opponent moves are also stored in userMoves but we cant send them back.
    const action: MovesMadeActionDto = {
      actionName: ActionNames.movesMade,
      moves: this.userMoves.filter((m) => m.color === myColor)
    };
    this.sendMessage(JSON.stringify(action));
    this.userMoves = [];
    this.dicesHistory = [];
    this.gameHistory = [];
  }

  shiftMoveAnimationsQueue(): void {
    // console.log('shifting animation queue');
    const clone = [...AppState.Singleton.moveAnimations.getValue()];
    clone.shift();
    AppState.Singleton.moveAnimations.setValue(clone);
  }

  sendMove(move: MoveDto): void {
    // removing next moves to decrease bytes.
    const action: OpponentMoveActionDto = {
      actionName: ActionNames.opponentMove,
      move: { ...move, nextMoves: [], animate: true }
    };
    this.sendMessage(JSON.stringify(action));
  }

  sendUndo(): void {
    const action: UndoActionDto = {
      actionName: ActionNames.undoMove
    };
    this.sendMessage(JSON.stringify(action));
  }

  ngOnDestroy(): void {
    const action: ConnectionInfoActionDto = {
      actionName: ActionNames.connectionInfo,
      connection: { pingMs: 0, connected: false }
    };
    this.sendMessage(JSON.stringify(action));
  }

  abortGame(): void {
    const action: AbortGameActionDto = {
      actionName: ActionNames.abortGame,
      gameId: AppState.Singleton.game.getValue().id
    };
    this.sendMessage(JSON.stringify(action));
  }
}
