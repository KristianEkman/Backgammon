export class CheckerArea {
  constructor(
    public x: number,
    public y: number,
    public width: number,
    public height: number,
    public pointIdx: number
  ) {}

  set(
    x: number,
    y: number,
    width: number,
    height: number,
    pointIdx: number
  ): void {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.pointIdx = pointIdx;
  }

  public contains(x: number, y: number): boolean {
    return (
      x >= this.x &&
      x <= this.x + this.width &&
      y >= this.y &&
      y <= this.y + this.height
    );
  }

  // private _checkers: Checker[] = [];
  // get checkers(): Checker[] {
  //   return this._checkers;
  // }

  drawBorder(cx: CanvasRenderingContext2D): void {
    cx.strokeRect(this.x, this.y, this.width, this.height);
    cx.fillText(this.pointIdx.toString(), this.x, this.y);
  }

  drawTop(cx: CanvasRenderingContext2D): void {
    cx.beginPath();
    const y = this.y;
    cx.moveTo(this.x, y);
    cx.lineTo(this.x + this.width, y);
    cx.closePath();
    cx.strokeStyle = '#28DD2E';
    cx.lineWidth = 2;
    cx.stroke();
  }

  drawBottom(cx: CanvasRenderingContext2D): void {
    const y = this.y + this.height;
    cx.moveTo(this.x, y);
    cx.lineTo(this.x + this.width, y);
    cx.closePath();
    cx.strokeStyle = '#28DD2E';
    cx.lineWidth = 2;
    cx.stroke();
  }
  hasValidMove = false;
  canBeMovedTo = false;
}