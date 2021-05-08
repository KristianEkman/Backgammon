export interface IThemes {
  boardBackground: string | CanvasGradient | CanvasPattern;
  homeBackground: string | CanvasGradient | CanvasPattern;
  border: string | CanvasGradient | CanvasPattern;
  whiteTriangle: string | CanvasGradient | CanvasPattern;
  blackTriangle: string | CanvasGradient | CanvasPattern;
  whiteChecker: string | CanvasGradient | CanvasPattern;
  blackChecker: string | CanvasGradient | CanvasPattern;
  textColor: string | CanvasGradient | CanvasPattern;
  highLight: string | CanvasGradient | CanvasPattern;
  checkerBorder: string | CanvasGradient | CanvasPattern;
}

export class DarkTheme implements IThemes {
  boardBackground = '#222';
  homeBackground = '#444';
  border = '#333';
  whiteTriangle = '#666';
  blackTriangle = '#333';
  whiteChecker = '#888';
  blackChecker = '#000';
  textColor = '#ddd';
  highLight = '#28DD2E';
  checkerBorder = '#777';
}
