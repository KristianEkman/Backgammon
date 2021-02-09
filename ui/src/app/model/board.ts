import { CheckerPoint } from './checker-point';

export class Board {
  constructor() {
    this._checkerPoints = [];
  }

  private _checkerPoints: CheckerPoint[];
  public get checkerPoints(): CheckerPoint[] {
    return this._checkerPoints;
  }
  public set checkerPoints(v: CheckerPoint[]) {
    this._checkerPoints = v;
  }
}
