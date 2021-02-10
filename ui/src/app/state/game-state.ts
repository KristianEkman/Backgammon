import { StateObject } from './state-object';

export class AppState {
  constructor() {
    this.busy = new StateObject<boolean>();
  }
  private static _singleton: AppState;
  public static get Singleton(): AppState {
    if (!this._singleton) {
      this._singleton = new AppState();
    }
    return this._singleton;
  }

  busy: StateObject<boolean>;
}
