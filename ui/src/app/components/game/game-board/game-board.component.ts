import {
  EventEmitter,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { MoveDto, GameDto, PlayerColor, GameState } from 'src/app/dto';
import { AppState } from 'src/app/state/app-state';
import { CheckerArea, CheckerDrag, Point, MoveAnimation } from './';

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
  @Input() myColor: PlayerColor | null = PlayerColor.black;
  @Input() dicesVisible: boolean | null = false;
  @Input() flipped = false;

  @Output() addMove = new EventEmitter<MoveDto>();
  @Output() moveAnimFinished = new EventEmitter<void>();

  borderWidth = 0;
  barWidth = 0;
  sideBoardWidth = 0;
  rectBase = 0;
  rectHeight = 0;
  checkerAreas: CheckerArea[] = [];
  blackHome: CheckerArea = new CheckerArea(0, 0, 0, 0, 25);
  whiteHome: CheckerArea = new CheckerArea(0, 0, 0, 0, 0);

  cx: CanvasRenderingContext2D | null = null;
  drawDirty = false;
  dragging: CheckerDrag | null = null;
  cursor: Point = new Point(0, 0);
  framerate = 60;
  animatedMove: MoveAnimation | undefined = undefined;
  animationSubscription: Subscription;
  hasTouch = false;

  constructor() {
    for (let r = 0; r < 26; r++) {
      this.checkerAreas.push(new CheckerArea(0, 0, 0, 0, 0));
    }

    this.animationSubscription = AppState.Singleton.moveAnimations
      .observe()
      .subscribe((moves: MoveDto[]) => {
        if (moves.length > 0 && this.animatedMove === undefined) {
          // console.log('starting animation ');
          this.animatedMove = new MoveAnimation(
            moves[0],
            this.getMoveStartPoint(moves[0]),
            this.getMoveEndPoint(moves[0]),
            () => {
              // finished callback
              this.animatedMove = undefined;
              this.drawDirty = true;
              this.moveAnimFinished.emit();
            }
          );
        }
      });
  }

  ngAfterViewInit(): void {
    if (!this.canvas) {
      return;
    }

    setInterval(() => {
      if ((this.drawDirty || this.animatedMove) && this.cx) {
        // console.log('drawing');
        this.draw(this.cx);
        this.drawDirty = false;
      }
    }, 1000 / this.framerate);

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    this.cx = canvasEl.getContext('2d');
    if (this.cx) {
      this.cx.translate(-0.5, -0.5);
    }
    this.drawDirty = true;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ([changes['width'] || changes['height']]) {
      this.recalculateGeometry();
    }
    this.drawDirty = true;
  }

  recalculateGeometry(): void {
    this.borderWidth = this.width * 0.01;
    this.barWidth = this.borderWidth * 2;
    this.sideBoardWidth = this.width * 0.1;
    this.rectBase =
      (this.width -
        this.barWidth -
        2 * this.borderWidth -
        this.sideBoardWidth * 2) /
      12;
    this.rectHeight = this.height * 0.42;

    //blacks bar
    this.checkerAreas[24].set(
      this.width / 2 - this.borderWidth,
      this.rectHeight / 2,
      this.borderWidth * 2,
      this.rectHeight / 2 + this.borderWidth,
      0
    );

    //whites bar
    this.checkerAreas[25].set(
      this.width / 2 - this.borderWidth,
      this.height / 2 + this.height * 0.08 - this.borderWidth,
      this.borderWidth * 2,
      this.rectHeight / 2,
      25
    );

    //blacks home
    this.blackHome.set(
      this.width - this.sideBoardWidth + 4,
      this.height - this.height * 0.44 - this.borderWidth - 4,
      this.sideBoardWidth / 2,
      this.height * 0.44,
      25
    );

    //white home
    this.whiteHome.set(
      this.width - this.sideBoardWidth + 4,
      this.borderWidth + 4,
      this.sideBoardWidth / 2,
      this.height * 0.44,
      0
    );

    let x = this.borderWidth + this.sideBoardWidth;
    let y = this.borderWidth;

    //Top triangles
    for (let i = 0; i < 12; i++) {
      if (i === 6) {
        x += this.barWidth;
      }
      this.checkerAreas[i].set(x, y, this.rectBase, this.rectHeight, 12 - i);
      x += this.rectBase;
    }

    //bottom
    x = this.borderWidth + this.sideBoardWidth;
    y = this.height - this.borderWidth - this.rectHeight;
    for (let i = 0; i < 12; i++) {
      if (i === 6) {
        x += this.barWidth;
      }

      this.checkerAreas[i + 12].set(
        x,
        y,
        this.rectBase,
        this.rectHeight,
        i + 13
      );
      x += this.rectBase;
    }
  }

  draw(cx: CanvasRenderingContext2D): void {
    if (!this.canvas) {
      return;
    }
    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    canvasEl.width = this.width;
    canvasEl.height = this.height;

    this.drawBoard(cx);
    // this.drawRects(cx); // debug
    this.drawCheckers(cx);
    if (this.animatedMove) {
      this.animatedMove.draw(cx, this.getCheckerWidth());
    }
  }

  drawRects(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }
    cx.lineWidth = 1;
    cx.fillStyle = '#000';
    cx.strokeStyle = '#F00';

    for (let r = 0; r < this.checkerAreas.length; r++) {
      this.checkerAreas[r].drawBorder(cx);
    }

    this.blackHome.drawBorder(cx);
    this.whiteHome.drawBorder(cx);
  }

  getMoveEndPoint(moveDto: MoveDto): Point {
    let rect: CheckerArea | undefined = undefined;
    if (moveDto.color === PlayerColor.black) {
      rect = this.checkerAreas.find((r) => r.pointIdx === moveDto.to);
      if (moveDto.to === 25) {
        rect = this.blackHome;
      }
    } else {
      rect = this.checkerAreas.find((r) => r.pointIdx === 25 - moveDto.to);
      if (moveDto.to === 25) {
        rect = this.whiteHome;
      }
    }
    if (rect) {
      const y = rect.y + rect.height / 2;
      const x = rect.x + this.getCheckerWidth();
      return new Point(x, y);
    }

    return new Point(0, 0);
  }

  getMoveStartPoint(moveDto: MoveDto): Point {
    if (!this.game) {
      return new Point(0, 0);
    }
    let pointIdx = 0;

    if (moveDto.color === PlayerColor.black) {
      pointIdx = moveDto.from;
    } else {
      pointIdx = 25 - moveDto.from;
    }
    const rect = this.checkerAreas.find((r) => r.pointIdx === pointIdx);
    if (rect) {
      const y = rect.y + rect.height / 2;
      const x = rect.x + this.getCheckerWidth();
      return new Point(x, y);
    }

    return new Point(0, 0);
  }

  getCheckerRadius(): number {
    return this.checkerAreas[0].width / 2;
  }

  getCheckerWidth(): number {
    return this.getCheckerRadius() * 0.87;
  }

  drawCheckers(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    if (!this.game) {
      return;
    }
    // console.log(this.game.points);

    const r = this.getCheckerRadius();
    const chWidth = this.getCheckerWidth();
    cx.lineWidth = 2;

    for (let p = 0; p < this.game.points.length; p++) {
      const point = this.game.points[p];
      let checkerCount = point.checkers.length;
      const area = this.checkerAreas.filter((r) => r.pointIdx === p)[0];

      //drawing the dragged checker
      const drawDrag = this.dragging && this.dragging.checkerArea == area;
      if (drawDrag) {
        checkerCount--;
        if (point.checkers[0].color === PlayerColor.black) {
          cx.fillStyle = '#000';
        } else {
          cx.fillStyle = '#fff';
        }
        cx.strokeStyle = '#28DD2E';
        cx.beginPath();
        cx.ellipse(
          this.cursor.x - chWidth / 2,
          this.cursor.y - chWidth / 2,
          chWidth,
          chWidth,
          0,
          0,
          360
        );
        // console.log('draw drag', this.cursor);

        cx.closePath();
        cx.fill();
        cx.stroke();
      }

      if (area.canBeMovedTo) {
        cx.beginPath();
        const y = p < 13 ? area.y - 3 : area.y + area.height + 3;
        cx.moveTo(area.x, y);
        cx.lineTo(area.x + area.width, y);
        cx.closePath();
        cx.strokeStyle = '#28DD2E';
        cx.lineWidth = 3;
        cx.stroke();
      }

      const dist = Math.min(2 * chWidth, area.height / checkerCount);

      cx.lineWidth = 1;
      const dragAnimationTo =
        this.animatedMove &&
        ((this.animatedMove?.move.color === PlayerColor.black &&
          this.animatedMove.move.to == p) ||
          (this.animatedMove?.move.color === PlayerColor.white &&
            this.animatedMove.move.to == 25 - p));

      if (dragAnimationTo) {
        checkerCount--;
      }

      for (let i = 0; i < checkerCount; i++) {
        const checker = point.checkers[i];
        // skipping checkers att home.
        if (
          (p === 0 && checker.color === PlayerColor.white) ||
          (p === 25 && checker.color === PlayerColor.black)
        ) {
          continue;
        }

        let x = area.x + r;
        if (p === 0 || p === 25) {
          x = area.x + chWidth / 2;
        }
        let y = 0;
        if (p < 13) {
          y = area.y + chWidth + dist * i;
        } else {
          y = area.y + area.height - chWidth - dist * i;
        }
        if (checker.color === PlayerColor.black) {
          cx.fillStyle = '#000';
          cx.strokeStyle = '#777';
        } else {
          cx.fillStyle = '#FFF';
          cx.strokeStyle = '#777';
        }
        cx.beginPath();
        cx.ellipse(x, y, chWidth, chWidth, 0, 0, 360);
        cx.closePath();
        cx.fill();
        cx.stroke();
        if (area.hasValidMove && i == checkerCount - 1 && !drawDrag) {
          cx.strokeStyle = '#28DD2E';
          cx.lineWidth = 1.5;
          cx.stroke();
        }
      }
    }

    // draw checkers reached home.
    const blackCount = this.game.points[25].checkers.filter(
      (c) => c.color === PlayerColor.black
    ).length;
    const whiteCount = this.game.points[0].checkers.filter(
      (c) => c.color === PlayerColor.white
    ).length;

    let x = this.width - this.sideBoardWidth + 4;
    let y = this.height - this.borderWidth - 4;
    cx.fillStyle = '#000';
    for (let i = 0; i < blackCount; i++) {
      cx.fillRect(x, y - i * 6, this.sideBoardWidth / 2, 5);
    }

    x = this.width - this.sideBoardWidth + 4;
    y = this.borderWidth + 4;
    cx.fillStyle = '#fff';
    for (let i = 0; i < whiteCount; i++) {
      cx.fillRect(x, y + i * 6, this.sideBoardWidth / 2, 5);
    }

    //draw homes if can be moved to
    if (this.blackHome.canBeMovedTo) {
      this.blackHome.drawBottom(cx);
    }

    if (this.whiteHome.canBeMovedTo) {
      this.whiteHome.drawTop(cx);
    }
  }

  //Draws the background and also set shape of all rectangles used for interaction.
  drawBoard(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    // color and line width
    cx.lineWidth = 1;

    cx.strokeStyle = '#000';
    const colors = ['#555', '#eee'];
    let colorIdx = 0;

    //Top triangles
    for (let i = 0; i < 12; i++) {
      const area = this.checkerAreas[i];
      const y = area.y;
      let x = area.x;

      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += area.width / 2;
      cx.lineTo(x, area.height);
      x += area.width / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }

    //bottom
    colorIdx = colorIdx === 0 ? 1 : 0;
    for (let i = 0; i < 12; i++) {
      const area = this.checkerAreas[i + 12];
      const y = area.y + area.height;
      let x = area.x;
      cx.fillStyle = colors[colorIdx];
      cx.beginPath();
      cx.moveTo(x, y);
      x += area.width / 2;
      cx.lineTo(x, y - area.height);
      x += area.width / 2;
      cx.lineTo(x, y);
      cx.closePath();
      cx.fill();
      colorIdx = colorIdx === 0 ? 1 : 0;
    }

    // the border
    cx.lineWidth = this.borderWidth;
    cx.strokeStyle = '#888';
    cx.strokeRect(
      this.sideBoardWidth + this.borderWidth / 2,
      this.borderWidth / 2,
      this.width - 2 * this.sideBoardWidth - this.borderWidth,
      this.height - this.borderWidth
    );

    //the bar
    cx.fillStyle = '#888';
    cx.fillRect(
      this.width / 2 - this.barWidth / 2,
      0,
      this.barWidth,
      this.height
    );
  }

  onMouseDown(event: MouseEvent): void {
    // console.log('mousedown');
    if (this.hasTouch) {
      return;
    }
    const point = this.getPoint(event);

    this.handleDown(point.x, point.y);
  }

  getPoint(event: MouseEvent): Point {
    // if (this.flipped) {
    //   return { x: event.offsetX, y: event.offsetY };
    // }
    // Cool that offsets are also rotated. Is that true on all browsers?
    return { x: event.offsetX, y: event.offsetY };
  }

  getTouchPoint(touch: Touch): Point {
    if (this.flipped) {
      return {
        x: this.width - touch.clientX + 10,
        y: this.height - touch.clientY + 25
        // todo, figure out this offset.
      };
    }
    return { x: touch.clientX, y: touch.clientY };
  }

  handleDown(clientX: number, clientY: number): void {
    if (!this.game) {
      return;
    }

    if (!this.canMove()) {
      return;
    }

    for (let i = 0; i < this.checkerAreas.length; i++) {
      const rect = this.checkerAreas[i];
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
        this.dragging = new CheckerDrag(rect, clientX, clientY, ptIdx);
        // console.log('dragging', this.dragging);
        break;
      }
    }
  }

  canMove(): boolean {
    if (!this.game) {
      return false;
    }
    if (this.myColor != this.game.currentPlayer) {
      return false;
    }
    if (!this.dicesVisible) {
      return false;
    }

    if (this.game.playState === GameState.ended) {
      return false;
    }

    return true;
  }

  onMouseMove(event: MouseEvent): void {
    // console.log('mousemove', event);
    const point = this.getPoint(event);
    this.handleMove(point.x, point.y);
  }

  handleMove(clientX: number, clientY: number): void {
    this.cursor.x = clientX;
    this.cursor.y = clientY;

    if (!this.game) {
      return;
    }

    if (!this.canMove()) {
      return;
    }

    if (this.dragging) {
      this.drawDirty = true;
      return;
    }

    this.setCanBeMovedTo(clientX, clientY);
  }

  setCanBeMovedTo(clientX: number, clientY: number): void {
    if (!this.game) {
      return;
    }

    // Indicating what can be moved and where.
    const isWhite = this.game.currentPlayer === PlayerColor.white;

    // resetting all
    this.checkerAreas.forEach((rect) => {
      rect.canBeMovedTo = false;
      rect.hasValidMove = false;
      this.whiteHome.canBeMovedTo = false;
      this.blackHome.canBeMovedTo = false;
    });

    for (let i = 0; i < this.checkerAreas.length; i++) {
      const rect = this.checkerAreas[i];
      const x = clientX - this.borderWidth;
      const y = clientY - this.borderWidth;
      if (!rect.contains(x, y)) {
        continue;
      }
      const ptIdx = isWhite ? 25 - rect.pointIdx : rect.pointIdx;
      const moves = this.game.validMoves.filter((m) => m.from === ptIdx);
      if (moves.length > 0) {
        rect.hasValidMove = true;
        moves.forEach((move) => {
          const toIdx = isWhite ? 25 - move.to : move.to;
          const area = this.checkerAreas.find((r) => r.pointIdx === toIdx);
          // not marking bar when checker is going home
          if (area && move.to < 25) {
            area.canBeMovedTo = true;
          }

          if (move.to === 25) {
            if (move.color === PlayerColor.white) {
              this.whiteHome.canBeMovedTo = true;
            } else {
              this.blackHome.canBeMovedTo = true;
            }
          }
        });
      }
    }
    this.drawDirty = true;
  }

  onMouseUp(event: MouseEvent): void {
    // console.log('mouse up', event);
    if (this.hasTouch) {
      return;
      // on mobile there is a mouse up event if the mouse hasn't been moved.
    }
    const point = this.getPoint(event);
    this.handleUp(point.x, point.y);
  }

  handleUp(clientX: number, clientY: number): void {
    if (!this.game) {
      return;
    }

    if (!this.canMove()) {
      return;
    }

    if (!this.dragging) {
      return;
    }

    const { xDown, yDown, fromIdx } = this.dragging;

    // Unless the cursor has moved to far, this is a click event, and should move the move of the largest dice.
    const isClick =
      Math.abs(clientX - xDown) < 3 && Math.abs(clientY - yDown) < 3;

    const allRects: CheckerArea[] = [...this.checkerAreas];
    if (this.game.currentPlayer === PlayerColor.black) {
      allRects.push(this.blackHome);
    } else {
      allRects.push(this.whiteHome);
    }

    for (let i = 0; i < allRects.length; i++) {
      const rect = allRects[i];
      const x = clientX - this.borderWidth;
      const y = clientY - this.borderWidth;
      if (!rect.contains(x, y)) {
        continue;
      }
      let ptIdx = rect.pointIdx;
      if (this.game?.currentPlayer === PlayerColor.white) {
        ptIdx = 25 - rect.pointIdx;
      }
      let move: MoveDto | undefined = undefined;
      if (isClick) {
        move = this.game.validMoves.find((m) => m.from === ptIdx);
      } else {
        move = this.game.validMoves.find(
          (m) => m.to === ptIdx && fromIdx === m.from
        );
      }
      if (move) {
        this.addMove.emit({ ...move, animate: isClick });
        break;
      }
    }
    this.drawDirty = true;
    // console.log('dragging null');
    this.dragging = null;
  }

  onTouchStart(event: TouchEvent): void {
    // console.log('touch start', event);
    this.hasTouch = true;
    if (event.touches.length !== 1) {
      return;
    }
    const touch = event.touches[0];
    const { x, y } = this.getTouchPoint(touch);
    this.lastTouch = { x, y };

    this.cursor.x = x;
    this.cursor.y = y;

    // console.log('touchstart', x, y);

    this.handleDown(x, y);
    this.setCanBeMovedTo(x, y);
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onTouchEnd(event: TouchEvent): void {
    // console.log('touchend', event);

    if (this.lastTouch != undefined) {
      this.handleUp(this.lastTouch.x, this.lastTouch.y);
    }
    this.lastTouch = undefined;
  }

  lastTouch: Point | undefined = undefined;
  onTouchMove(event: TouchEvent): void {
    // console.log('touchmove');
    if (event.touches.length !== 1) {
      return;
    }
    const touch = event.touches[0];
    const { x, y } = this.getTouchPoint(touch);

    this.lastTouch = { x, y };

    this.handleMove(x, y);
  }
}
