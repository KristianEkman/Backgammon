export interface IThemes {
  name: string;
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
  name = 'dark';
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

export class LightTheme implements IThemes {
  name = 'light';
  boardBackground = '#808080';
  homeBackground = '#444';
  border = '#333';
  whiteTriangle = '#666';
  blackTriangle = '#444';
  whiteChecker = '#aaa';
  blackChecker = '#000';
  textColor = '#222';
  highLight = '#28DD2E';
  checkerBorder = '#777';
}

export class BlueTheme implements IThemes {
  name = 'blue';
  boardBackground = '#808096';
  homeBackground = '#445';
  border = '#334';
  whiteTriangle = '#667';
  blackTriangle = '#334';
  whiteChecker = '#aaa';
  blackChecker = '#000';
  textColor = '#223';
  highLight = '#28DD2E';
  checkerBorder = '#778';
}

export class PinkTheme implements IThemes {
  name = 'pink';
  boardBackground = '#968080';
  homeBackground = '#544';
  border = '#433';
  whiteTriangle = '#766';
  blackTriangle = '#433';
  whiteChecker = '#aaa';
  blackChecker = '#000';
  textColor = '#322';
  highLight = '#28DD2E';
  checkerBorder = '#877';
}
