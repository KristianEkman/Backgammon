import { AppState } from 'src/app/state/app-state';

export class Theme {
  public static Themes = ['dark', 'light', 'blue', 'pink', 'green'];

  public static change(theme: string): void {
    if (!theme || theme.length === 0) theme = 'dark';
    this.Themes.forEach((v) => {
      document.body.classList.remove(v);
    });
    document.body.classList.add(theme);
    AppState.Singleton.theme.setValue(theme);
  }
}
