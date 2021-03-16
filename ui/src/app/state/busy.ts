import { AppState } from './app-state';

export class Busy {
  constructor(public text: string, public overlay: boolean) {}

  static show(): void {
    AppState.Singleton.busy.setValue(new Busy('Please wait', true));
  }

  static hide(): void {
    AppState.Singleton.busy.clearValue();
  }

  static showNoOverlay(): void {
    AppState.Singleton.busy.setValue(new Busy('Please wait', false));
  }
}
