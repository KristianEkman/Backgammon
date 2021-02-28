import { MoveDto, PlayerColor } from 'src/app/dto';
import { Point } from './';

export class MoveAnimation {
  constructor(
    public move: MoveDto,
    private from: Point,
    private to: Point,
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
    const { x, y } = this.currentPos;
    if (this.move.color === PlayerColor.black) {
      cx.fillStyle = '#000';
      cx.strokeStyle = '#FFF';
    } else {
      cx.fillStyle = '#FFF';
      cx.strokeStyle = '#000';
    }
    cx.beginPath();
    cx.ellipse(x, y, width, width, 0, 0, 360);
    cx.closePath();
    cx.fill();
    cx.stroke();
  }
}
