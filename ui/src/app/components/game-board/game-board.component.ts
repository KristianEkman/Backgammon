import { OnChanges, ViewChild } from '@angular/core';
import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
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

  borderWidth = 8;
  barWidth = this.borderWidth * 2;
  rectBase = 0;
  rectHeight = 0;
  rectangles: Rectangle[] = [];
  cx: CanvasRenderingContext2D | null = null;
  drawDirty = false;

  ngAfterViewInit(): void {
    if (!this.canvas) {
      return;
    }

    setInterval(() => {
      console.log('draw', this.drawDirty);
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
  }

  drawCheckers(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    if (!this.game) {
      return;
    }

    console.log('draw checkers');
    this.game.points.forEach((point) => {
      const rect = this.rectangles[point.blackNumber];
      for (let i = 0; i < point.checkers.length; i++) {
        const checker = point.checkers[i];
        if (checker.color === PlayerColor.black) {
          cx.fillStyle = '#000';
        } else {
          cx.fillStyle = '#FFF';
        }
        cx.beginPath();
        cx.ellipse(rect.x, rect.y, rect.width, rect.width, 0, 0, 360);
        cx.closePath();
        cx.fill();
      }
    });
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
    for (let i = 0; i < 12; i++) {
      if (i == 6) {
        x += this.barWidth;
      }
      this.rectangles.push(
        new Rectangle(x, y, this.rectBase, this.rectHeight, 12 - i)
      );
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
      this.rectangles.push(
        new Rectangle(
          x,
          y - this.rectHeight,
          this.rectBase,
          this.rectHeight,
          i + 13
        )
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

  onMouseDown(event: MouseEvent): void {
    const { clientX, clientY } = event;
    this.rectangles.forEach((rect) => {
      if (
        rect.contains(clientX - this.borderWidth, clientY - this.borderWidth)
      ) {
        console.log(clientX, clientY, rect.pointIdx);
      }
    });
  }
}
