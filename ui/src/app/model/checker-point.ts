import { Checker } from './checker';

export class CheckerPoint {
  constructor() {
    this._Checkers = [];
  }

  private _Checkers: Checker[];
  public get Checkers(): Checker[] {
    return this._Checkers;
  }
  public set Checkers(v: Checker[]) {
    this._Checkers = v;
  }
}
