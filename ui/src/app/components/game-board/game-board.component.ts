import { EventEmitter, OnChanges, Output, ViewChild } from '@angular/core';
import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
import { MoveDto } from 'src/app/dto';
import { GameDto } from 'src/app/dto/gameDto';
import { PlayerColor } from 'src/app/dto/playerColor';
import { Rectangle } from 'src/app/utils/rectangle';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.scss']
})
export class GameBoardComponent implements AfterViewInit, OnChanges {
  @ViewChild('canvas') public canvas: ElementRef | undefined;

  @Input() public width = 600;
  @Input() public height = 400;
  @Input() game: GameDto | null = null;
  @Output() addMove = new EventEmitter<MoveDto>();

  borderWidth = 8;
  barWidth = this.borderWidth * 2;
  rectBase = 0;
  rectHeight = 0;
  rectangles: Rectangle[] = [];
  cx: CanvasRenderingContext2D | null = null;
  drawDirty = false;
  constructor() {
    for (let r = 0; r < 26; r++) {
      this.rectangles.push(new Rectangle(0, 0, 0, 0, 0));
    }
  }

  ngAfterViewInit(): void {
    if (!this.canvas) {
      return;
    }

    setInterval(() => {
      if (this.drawDirty) {
        this.draw(this.cx);
        this.drawDirty = false;
      }
    }, 200);

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    canvasEl.width = this.width;
    canvasEl.height = this.height;

    this.cx = canvasEl.getContext('2d');
    this.drawBoard(this.cx);
  }

  ngOnChanges(): void {
    this.drawDirty = true;
  }

  draw(cx: CanvasRenderingContext2D | null): void {
    this.drawBoard(cx);
    this.drawCheckers(cx);
    this.drawTurn(cx);
    this.drawRects(cx);
  }

  drawRects(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }
    cx.lineWidth = 1;
    cx.fillStyle = '#000';
    cx.strokeStyle = '#F00';

    for (let r = 0; r < this.rectangles.length; r++) {
      const rect = this.rectangles[r];
      cx?.strokeRect(rect.x, rect.y, rect.width, rect.height);
      cx.fillText(rect.pointIdx.toString(), rect.x, rect.y);
    }
  }

  drawCheckers(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    if (!this.game) {
      return;
    }
    // console.log(this.game.points);

    const r = this.rectangles[0].width / 2;
    const chWidth = r * 0.67;

    // blacks bar
    for (let i = 0; i < this.game.points[0].checkers.length; i++) {
      cx.beginPath();
      cx.ellipse(this.width / 2, this.height / 4, chWidth, chWidth, 0, 0, 360);
      cx.closePath();
      cx.fillStyle = '#000';
      cx.fill();
    }

    // whites bar
    for (let i = 0; i < this.game.points[25].checkers.length; i++) {
      cx.beginPath();
      const w = this.width / 2;
      const h = this.height * 0.75;
      cx.ellipse(w, h, chWidth, chWidth, 0, 0, 360);
      cx.closePath();
      cx.fillStyle = '#FFF';
      cx.fill();
    }

    for (let p = 1; p < this.game.points.length - 1; p++) {
      const point = this.game.points[p];
      const checkerCount = point.checkers.length;
      const rect = this.rectangles.filter((r) => r.pointIdx === p)[0];

      const dist = Math.min(2 * chWidth, rect.height / checkerCount);

      for (let i = 0; i < checkerCount; i++) {
        const checker = point.checkers[i];
        const x = rect.x + r;
        let y = 0;
        if (p < 13) {
          y = rect.y + chWidth + dist * i;
        } else {
          y = rect.y + rect.height - chWidth - dist * i;
        }
        if (checker.color === PlayerColor.black) {
          cx.fillStyle = '#000';
        } else {
          cx.fillStyle = '#FFF';
        }
        cx.beginPath();
        cx.ellipse(x, y, chWidth, chWidth, 0, 0, 360);
        cx.closePath();
        cx.fill();
      }
    }
  }

  drawBoard(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    // color and line width
    cx.lineWidth = 1;
    cx.fillStyle = '#ccc';
    cx.fillRect(0, 0, this.width, this.height);

    cx.strokeStyle = '#000';
    this.rectBase = (this.width - this.barWidth - 2 * this.borderWidth) / 12;
    this.rectHeight = this.height * 0.42;
    const colors = ['#555', '#eee'];
    let colorIdx = 0;
    let x = this.borderWidth;
    let y = this.borderWidth;

    //blacks bar
    this.rectangles[24].set(
      this.width / 2 - this.borderWidth,
      this.rectHeight / 2,
      this.borderWidth * 2,
      this.rectHeight / 2 + this.borderWidth,
      0
    );

    //blacks bar
    this.rectangles[25].set(
      this.width / 2 - this.borderWidth,
      this.height / 2 + this.height * 0.08 - this.borderWidth,
      this.borderWidth * 2,
      this.rectHeight / 2,
      25
    );

    for (let i = 0; i < 12; i++) {
      if (i == 6) {
        x += this.barWidth;
      }
      this.rectangles[i].set(x, y, this.rectBase, this.rectHeight, 12 - i);
      // cx.strokeRect(x, y, this.rectBase, this.rectHeight);

      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += this.rectBase / 2;
      cx.lineTo(x, this.rectHeight);
      x += this.rectBase / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }

    //bottom
    colorIdx = colorIdx === 0 ? 1 : 0;
    y = this.height - this.borderWidth;
    x = this.borderWidth;
    for (let i = 0; i < 12; i++) {
      if (i == 6) {
        x += this.barWidth;
      }

      this.rectangles[i + 12].set(
        x,
        y - this.rectHeight,
        this.rectBase,
        this.rectHeight,
        i + 13
      );
      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += this.rectBase / 2;
      cx.lineTo(x, y - this.rectHeight);
      x += this.rectBase / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }

    cx.lineWidth = this.borderWidth * 2;
    cx.strokeStyle = '#888';
    cx.strokeRect(0, 0, this.width, this.height);
    cx.fillStyle = '#888';
    cx.fillRect(
      this.width / 2 - this.barWidth / 2,
      0,
      this.barWidth,
      this.height
    );
  }

  drawTurn(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }
    let text = '';
    cx.fillStyle = '#000';
    cx.font = '14px Arial';
    if (!this.game) {
      text = 'Waiting for opponent to connect';
    } else if (this.game.myColor == this.game.currentPlayer) {
      text = `Your turn to move.  (${PlayerColor[this.game.currentPlayer]})`;
    } else {
      text = `Waiting for ${PlayerColor[this.game.currentPlayer]} to move.`;
    }

    cx.fillText(text, 40, this.height / 2);
  }

  onMouseDown(event: MouseEvent): void {
    if (!this.game) {
      return;
    }

    if (this.game.myColor != this.game.currentPlayer) {
      return;
    }

    const { clientX, clientY } = event;
    for (let i = 0; i < this.rectangles.length; i++) {
      const rect = this.rectangles[i];
      const x = clientX - this.borderWidth;
      const y = clientY - this.borderWidth;
      if (!rect.contains(x, y)) {
        continue;
      }
      let ptIdx = rect.pointIdx;
      if (this.game?.currentPlayer === PlayerColor.white) {
        ptIdx = 25 - rect.pointIdx;
      }
      // The moves are ordered  by backend by dice value.
      const move = this.game.validMoves.find((m) => m.from === ptIdx);
      if (move !== undefined) {
        this.addMove.emit(move);
      }
    }
  }
}
