import { ViewChild } from '@angular/core';
import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.scss']
})
export class GameBoardComponent implements AfterViewInit {
  @ViewChild('canvas') public canvas: ElementRef | undefined;

  @Input() public width = 600;
  @Input() public height = 400;

  ngAfterViewInit(): void {
    if (!this.canvas) {
      return;
    }

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    canvasEl.width = this.width;
    canvasEl.height = this.height;

    const cx = canvasEl.getContext('2d');
    this.drawBoard(cx);
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

    const borderWidth = 8;
    const barWidth = borderWidth * 2;
    const rectBase = (this.width - barWidth - 2 * borderWidth) / 12;
    const rectHeight = this.height * 0.42;
    const colors = ['#555', '#eee'];
    let colorIdx = 0;
    let x = borderWidth;
    let y = borderWidth;
    for (let i = 0; i < 12; i++) {
      if (i == 6) {
        x += barWidth;
      }
      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += rectBase / 2;
      cx.lineTo(x, rectHeight);
      x += rectBase / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }
    colorIdx = colorIdx === 0 ? 1 : 0;
    y = this.height - borderWidth;
    x = borderWidth;
    for (let i = 0; i < 12; i++) {
      if (i == 6) {
        x += barWidth;
      }
      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += rectBase / 2;
      cx.lineTo(x, y - rectHeight);
      x += rectBase / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }

    cx.lineWidth = borderWidth * 2;
    cx.strokeStyle = '#888';
    cx.strokeRect(0, 0, this.width, this.height);
    cx.fillStyle = '#888';
    cx.fillRect(this.width / 2 - barWidth / 2, 0, barWidth, this.height);
  }
}
