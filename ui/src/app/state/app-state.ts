import { StatusMessage } from '../dto/local/status-message';
import {
  ConnectionDto,
  GameState,
  MoveDto,
  PlayerColor,
  UserDto
} from '../dto';
import { DiceDto } from '../dto/diceDto';
import { GameDto } from '../dto/gameDto';
import { StateObject } from './state-object';

export class AppState {
  constructor() {
    this.busy = new StateObject<boolean>();
    this.game = new StateObject<GameDto>();
    this.myColor = new StateObject<PlayerColor>();
    this.myColor.setValue(PlayerColor.neither);
    this.dices = new StateObject<DiceDto[]>();
    this.dices.setValue([]);
    this.moveAnimations = new StateObject<MoveDto[]>();
    this.moveAnimations.setValue([]);
    this.myConnection = new StateObject<ConnectionDto>();
    this.opponentConnection = new StateObject<ConnectionDto>();
    this.user = new StateObject<UserDto>();
    this.statusMessage = new StateObject<StatusMessage>();
    this.moveTimer = new StateObject<number>();
  }
  private static _singleton: AppState;
  public static get Singleton(): AppState {
    if (!this._singleton) {
      this._singleton = new AppState();
    }
    return this._singleton;
  }

  busy: StateObject<boolean>;
  game: StateObject<GameDto>;
  myColor: StateObject<PlayerColor>;
  dices: StateObject<DiceDto[]>;
  moveAnimations: StateObject<MoveDto[]>;
  myConnection: StateObject<ConnectionDto>;
  opponentConnection: StateObject<ConnectionDto>;
  user: StateObject<UserDto>;
  statusMessage: StateObject<StatusMessage>;
  moveTimer: StateObject<number>;

  myTurn(): boolean {
    const game = this.game.getValue();

    return (
      game &&
      game.playState !== GameState.ended &&
      game.currentPlayer === this.myColor.getValue()
    );
  }
}
