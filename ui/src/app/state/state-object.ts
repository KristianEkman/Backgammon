/* eslint-disable @typescript-eslint/no-explicit-any */
import { Observable, ReplaySubject } from 'rxjs';

/*
  Represents an object which raises a changed ReplaySubject when its value is changed.
  The StateObjects value is made immutable.
*/
export class StateObject<T> {
  constructor() {
    this._replaySubject = new ReplaySubject<T>();
  }

  private _replaySubject: ReplaySubject<T>;
  public observe(): Observable<T> {
    return this._replaySubject.asObservable();
  }

  private _value: any;
  public getValue(): T {
    return this._value;
  }

  public setValue(v: T): void {
    this._value = v;
    if (v) {
      StateObject.deepFreeze(this._value);
    }
    this._replaySubject.next(v);
  }

  public clearValue(): void {
    this._value = undefined;
    this._replaySubject.next(this._value);
  }

  private static deepFreeze(obj: any): any {
    Object.freeze(obj);
    if (obj === undefined) {
      return obj;
    }

    Object.getOwnPropertyNames(obj).forEach(function (prop) {
      if (
        obj[prop] !== null &&
        (typeof obj[prop] === 'object' || typeof obj[prop] === 'function') &&
        !Object.isFrozen(obj[prop])
      ) {
        StateObject.deepFreeze(obj[prop]);
      }
    });

    return obj;
  }
}
