import { MoveDto, PlayerColor } from 'src/app/dto';
import { Point } from './';
import { Checker } from './checker';
import { IThemes } from './themes';

export class MoveAnimation {
  constructor(
    public move: MoveDto,
    private from: Point,
    private to: Point,
    private theme: IThemes,
    private flipped: boolean,
    finished: (move: MoveDto) => void
  ) {
    this.incrementX = (to.x - from.x) / this.frames;
    this.incrementY = (to.y - from.y) / this.frames;
    this.currentPos = { ...from };
    const timerID = setInterval(() => {
      this.currentFrame++;
      this.currentPos = {
        x: this.from.x + this.incrementX * this.currentFrame,
        y: this.from.y + this.incrementY * this.currentFrame
      };
      if (this.currentFrame >= this.frames) {
        clearInterval(timerID);
        finished(this.move);
      }
    }, 20);
  }

  frames = 20;
  currentFrame = 0;
  incrementX: number;
  incrementY: number;
  currentPos: Point;

  draw(cx: CanvasRenderingContext2D, width: number): void {
    Checker.draw(
      cx,
      this.currentPos,
      width,
      this.theme,
      this.move.color,
      false,
      true,
      this.flipped
    );
    // if (this.move.color === PlayerColor.black) {
    //   cx.fillStyle = '#000';
    // } else {
    //   cx.fillStyle = '#FFF';
    // }
    // cx.beginPath();
    // cx.ellipse(x, y, width, width, 0, 0, Math.PI * 2);
    // cx.closePath();
    // cx.fill();
  }
}
