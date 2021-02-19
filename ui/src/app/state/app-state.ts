import { MoveDto, PlayerColor } from '../dto';
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

  myTurn(): boolean {
    return this.game.getValue().currentPlayer === this.myColor.getValue();
  }
}
