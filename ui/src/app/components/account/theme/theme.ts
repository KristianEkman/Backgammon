export class Theme {
  public static Themes = ['light', 'dark'];

  public static change(theme: string): void {
    if (!theme || theme.length === 0) theme = 'black';
    this.Themes.forEach((v) => {
      document.body.classList.remove(v);
    });
    document.body.classList.add(theme);
  }
}
