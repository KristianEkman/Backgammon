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
import { Checker } from './checker';
import { DarkTheme, IThemes } from './themes';

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
  checkerAreas: CheckerArea[] = [];
  blackHome: CheckerArea = new CheckerArea(0, 0, 0, 0, 25);
  whiteHome: CheckerArea = new CheckerArea(0, 0, 0, 0, 0);
  blackBar: CheckerArea = new CheckerArea(0, 0, 0, 0, 0);
  whiteBar: CheckerArea = new CheckerArea(0, 0, 0, 0, 25);

  cx: CanvasRenderingContext2D | null = null;
  dragging: CheckerDrag | null = null;
  cursor: Point = new Point(0, 0);
  framerate = 60;
  animatedMove: MoveAnimation | undefined = undefined;
  animationSubscription: Subscription;
  hasTouch = false;
  whitesName = '';
  blacksName = '';
  theme: IThemes = new DarkTheme();

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
            this.theme,
            this.flipped,
            () => {
              // finished callback
              this.animatedMove = undefined;
              this.moveAnimFinished.emit();
            },
            () => {
              this.requestDraw();
            }
          );
        }
      });
  }

  ngAfterViewInit(): void {
    if (!this.canvas) {
      return;
    }

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    this.cx = canvasEl.getContext('2d');
    this.requestDraw();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ([changes['width'] || changes['height']]) {
      this.recalculateGeometry();
    }
    this.requestDraw();
    const bName =
      this.myColor === PlayerColor.black ? 'You' : this.game?.blackPlayer.name;
    const wName =
      this.myColor === PlayerColor.white ? 'You' : this.game?.whitePlayer.name;
    const bLeft = this.game?.blackPlayer.pointsLeft;
    const wLeft = this.game?.whitePlayer.pointsLeft;

    this.blacksName = this.game ? `${bName} - ${bLeft} left` : '';
    this.whitesName = this.game ? `${wName} - ${wLeft} left` : '';
    // console.log(this.game?.playState);
  }

  recalculateGeometry(): void {
    this.borderWidth = this.width * 0.01;
    this.barWidth = this.borderWidth * 3;
    this.sideBoardWidth = this.width * 0.1;

    const rectH = this.height * 0.42;

    //blacks bar
    this.checkerAreas[24].set(
      this.width / 2 - this.borderWidth * 1.5,
      rectH / 2,
      this.borderWidth * 3,
      rectH / 2 + this.borderWidth,
      0
    );
    this.blackBar = this.checkerAreas[24];

    //whites bar
    this.checkerAreas[25].set(
      this.width / 2 - this.borderWidth * 1.5,
      this.height / 2 + this.height * 0.08 - this.borderWidth,
      this.borderWidth * 3,
      rectH / 2,
      25
    );

    this.whiteBar = this.checkerAreas[25];

    //blacks home
    this.blackHome.set(
      this.width - this.sideBoardWidth - this.borderWidth / 2,
      this.height - this.height * 0.44 - this.borderWidth / 2,
      this.getCheckerWidth() * 2 + this.borderWidth + 2,
      this.height * 0.44,
      25
    );

    //white home
    this.whiteHome.set(
      this.width - this.sideBoardWidth - this.borderWidth / 2,
      this.borderWidth / 2,
      this.getCheckerWidth() * 2 + this.borderWidth + 2,
      this.height * 0.44,
      0
    );

    let x = this.borderWidth + this.sideBoardWidth;
    let y = this.borderWidth;
    const rectBase = this.getRectBase();

    //Top triangles
    for (let i = 0; i < 12; i++) {
      if (i === 6) {
        x += this.barWidth;
      }
      this.checkerAreas[i].set(x, y, rectBase, rectH, 12 - i);
      x += rectBase;
    }

    //bottom triangles
    x = this.borderWidth + this.sideBoardWidth;
    y = this.height - this.borderWidth - rectH;
    for (let i = 0; i < 12; i++) {
      if (i === 6) {
        x += this.barWidth;
      }

      this.checkerAreas[i + 12].set(x, y, rectBase, rectH, i + 13);
      x += rectBase;
    }
  }
  getRectBase(): number {
    return (
      (this.width -
        this.barWidth -
        this.borderWidth * 2 -
        this.sideBoardWidth * 2) /
      12
    );
  }

  requestDraw(): void {
    requestAnimationFrame(this.draw.bind(this));
  }

  draw(): number {
    if (!this.canvas || !this.cx) {
      return 0;
    }

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    canvasEl.width = this.width;
    canvasEl.height = this.height;
    const cx = this.cx;

    // this.drawIcon(cx);

    this.drawBoard(cx);
    // this.drawDebugRects(cx);
    this.drawCheckers(cx);
    if (this.animatedMove) {
      this.animatedMove.draw(cx, this.getCheckerWidth());
    }
    return 0;
  }

  drawIcon(cx: CanvasRenderingContext2D): void {
    if (!this.game) {
      return;
    }
    cx.fillStyle = '#fff';
    cx.fillRect(0, 0, 500, 500);

    Checker.draw(
      cx,
      { x: 100, y: 100 },
      40,
      this.theme,
      PlayerColor.black,
      false,
      false,
      false
    );
    cx.font = 'bold 50px Arial';
    cx.fillStyle = '#ccc';
    cx.fillText('B', 85, 118);
  }

  drawDebugRects(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }
    cx.lineWidth = 1;
    cx.strokeStyle = '#850';
    cx.fillStyle = '#850';

    for (let r = 0; r < this.checkerAreas.length; r++) {
      this.checkerAreas[r].drawBorder(cx, true);
    }

    this.blackHome.drawBorder(cx, true);
    this.whiteHome.drawBorder(cx, true);
  }

  getMoveEndPoint(moveDto: MoveDto): Point {
    if (!this.game) {
      return new Point(0, 0);
    }
    let pointIdx = 0;

    if (moveDto.color === PlayerColor.black) {
      if (moveDto.to === 0) {
        return this.blackBar.getCenter();
      }
      if (moveDto.to === 25) {
        return this.blackHome.getCenter();
        // black bar
      }
    } else {
      if (moveDto.to === 0) {
        return this.whiteBar.getCenter();
      }
      if (moveDto.to === 25) {
        return this.whiteHome.getCenter();
      }
    }

    if (moveDto.color === PlayerColor.black) {
      pointIdx = moveDto.to;
    } else {
      // white move
      pointIdx = 25 - moveDto.to;
    }

    const rect = this.checkerAreas.find((r) => r.pointIdx === pointIdx);
    const checkCount = this.game.points[pointIdx].checkers.length;
    const cw = this.getCheckerWidth() * 2;
    if (rect) {
      let y = rect.y + checkCount * cw + cw / 2;
      if (pointIdx > 12) {
        y = rect.y + rect.height - checkCount * cw - cw / 2;
      }
      const x = rect.x + cw;
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
      if (moveDto.from === 0) {
        return this.blackBar.getCenter();
      }
      if (moveDto.from === 25) {
        return this.blackHome.getCenter();
        // black bar
      }
    } else {
      if (moveDto.from === 0) {
        return this.whiteBar.getCenter();
      }
      if (moveDto.from === 25) {
        return this.whiteHome.getCenter();
      }
    }

    if (moveDto.color === PlayerColor.black) {
      pointIdx = moveDto.from;
    } else {
      // white move
      pointIdx = 25 - moveDto.from;
    }
    const rect = this.checkerAreas.find((r) => r.pointIdx === pointIdx);
    const checkCount = this.game.points[pointIdx].checkers.length;
    const cw = this.getCheckerWidth() * 2;
    if (rect) {
      let y = rect.y + checkCount * (cw - 1) - cw / 2;
      if (pointIdx > 12) {
        y = rect.y + rect.height - checkCount * (cw - 1) + cw / 2;
      }
      const x = rect.x + cw;
      return new Point(x, y);
    }

    return new Point(0, 0);
  }

  getCheckerRadius(): number {
    return this.getRectBase() / 2;
  }

  getCheckerWidth(): number {
    return this.getCheckerRadius() * 0.8;
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

    for (let p = 0; p < this.game.points.length; p++) {
      const point = this.game.points[p];
      let checkerCount = point.checkers.length;
      const area = this.checkerAreas.filter((r) => r.pointIdx === p)[0];

      //drawing the dragged checker
      const drawDrag = this.dragging && this.dragging.checkerArea == area;
      if (drawDrag) {
        checkerCount--;
      }

      if (area.canBeMovedTo) {
        const y = p < 13 ? area.y - 3 : area.y + area.height + 3;
        cx.beginPath();
        cx.moveTo(area.x + 5, y);

        cx.lineTo(area.x + area.width - 10, y);
        cx.closePath();
        cx.strokeStyle = this.theme.highLight;
        cx.lineWidth = 3;
        cx.stroke();
      }

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

      // distance between checkers on a point
      const dist = Math.min(2 * chWidth, area.height / checkerCount);
      // main draw checkers loop
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
        if ((p > 0 && p < 13) || p === 25) {
          // blacks bar and top triangles are drawn bottom down.
          y = area.y + chWidth + dist * i + 2;
        } else {
          // whites bar and top triangles are drawn bottom down.
          y = area.y + area.height - chWidth - dist * i - 2;
        }
        const highLight =
          area.hasValidMove && i == checkerCount - 1 && !drawDrag;
        Checker.draw(
          cx,
          { x, y },
          chWidth,
          this.theme,
          checker.color,
          highLight,
          false,
          this.flipped
        );
      }
    }

    if (this.dragging) {
      Checker.draw(
        cx,
        this.cursor,
        chWidth,
        this.theme,
        this.dragging.color,
        false,
        true,
        this.flipped
      );
    }
    this.drawHomes(cx);
  }

  drawHomes(cx: CanvasRenderingContext2D): void {
    // draw checkers at  home.
    if (!this.game) {
      return;
    }
    // black
    const blackCount = this.game.points[25].checkers.filter(
      (c) => c.color === PlayerColor.black
    ).length;
    const bw = this.borderWidth;

    let x = this.blackHome.x + bw;
    let y = this.blackHome.y + this.blackHome.height - bw * 1.5;
    cx.fillStyle = this.theme.blackChecker;

    const maxHblack = (this.blackHome.height - 2 * bw) / blackCount - 1;
    const chb = Math.min(5, maxHblack);

    for (let i = 0; i < blackCount; i++) {
      cx.fillRect(x, y - i * (chb + 1), this.sideBoardWidth / 2 - bw, chb);
    }

    // white
    const whiteCount = this.game.points[0].checkers.filter(
      (c) => c.color === PlayerColor.white
    ).length;

    x = this.whiteHome.x + bw;
    y = this.whiteHome.y + bw;
    cx.fillStyle = this.theme.whiteChecker;

    const maxHwhite = (this.whiteHome.height - 2 * bw) / whiteCount - 1;
    const chw = Math.min(5, maxHwhite);

    for (let i = 0; i < whiteCount; i++) {
      cx.fillRect(x, y + i * (chw + 1), this.sideBoardWidth / 2 - bw, chw);
    }

    //draw homes if can be moved to
    if (this.blackHome.canBeMovedTo) {
      this.blackHome.highLightBottom(cx);
    }

    if (this.whiteHome.canBeMovedTo) {
      this.whiteHome.highLightTop(cx);
    }
  }

  //Draws the background and also set shape of all rectangles used for interaction.
  drawBoard(cx: CanvasRenderingContext2D | null): void {
    if (!cx) {
      return;
    }

    cx.fillStyle = this.theme.boardBackground;
    cx.fillRect(
      this.sideBoardWidth,
      0,
      this.width - this.sideBoardWidth * 2,
      this.height
    );
    // color and line width
    cx.lineWidth = 1;
    const colors = [this.theme.blackTriangle, this.theme.whiteTriangle];
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

    //Bottom triangles
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

    this.drawBorders(cx);

    //the bar
    cx.fillStyle = this.theme.border;
    cx.fillRect(
      this.width / 2 - this.barWidth / 2,
      2,
      this.barWidth,
      this.height - 4
    );
  }

  drawBorders(cx: CanvasRenderingContext2D): void {
    cx.strokeStyle = this.theme.border;
    cx.lineWidth = this.borderWidth;
    this.whiteHome.fill(cx, this.theme.boardBackground);
    this.whiteHome.drawBorder(cx, false);
    this.blackHome.fill(cx, this.theme.boardBackground);
    this.blackHome.drawBorder(cx, false);
    const homeFntSize = this.blackHome.width / 2 + 'px Arial';
    // names of players
    cx.font = '18px Arial';
    cx.fillStyle = this.theme.textColor;
    cx.save();
    cx.translate(this.blackHome.x, this.blackHome.y);
    cx.rotate(Math.PI / 2);
    cx.fillStyle = this.theme.textColor;
    cx.fillText(this.blacksName, 0, -this.blackHome.width - 11);
    cx.fillStyle = this.theme.border;
    cx.font = homeFntSize;
    cx.fillText(
      'Black',
      this.borderWidth / 2 + 2,
      -this.borderWidth / 2 - 2,
      this.blackHome.height
    );
    cx.restore();

    cx.save();
    cx.translate(this.whiteHome.x, this.whiteHome.y);
    cx.rotate(Math.PI / 2);
    cx.font = '18px Arial';
    cx.fillStyle = this.theme.textColor;
    cx.fillText(this.whitesName, 0, -this.whiteHome.width - 11);
    cx.fillStyle = this.theme.border;
    cx.font = homeFntSize;

    cx.fillText(
      'White',
      this.borderWidth / 2 + 2,
      -this.borderWidth / 2 - 2,
      this.whiteHome.height
    );
    cx.restore();

    // the border
    cx.lineWidth = this.borderWidth;
    cx.strokeRect(
      this.sideBoardWidth + this.borderWidth / 2,
      this.borderWidth / 2,
      this.width - 2 * this.sideBoardWidth - this.borderWidth,
      this.height - this.borderWidth
    );

    // 3d effect of borders
    const sbw = this.sideBoardWidth;
    const bow = this.borderWidth;
    const hbw = bow / 2;
    const h = this.height;
    const w = this.width;
    cx.beginPath();
    cx.strokeStyle = !this.flipped ? '#444' : '#222';
    cx.lineWidth = 2;
    cx.moveTo(sbw, h);
    cx.lineTo(sbw, 1);

    cx.lineTo(sbw + w - this.blackHome.width * 2 - bow * 2, 1);
    cx.moveTo(sbw + bow, h - bow);
    cx.lineTo(w - sbw - bow, h - bow);
    cx.moveTo(w / 2 - this.barWidth / 2 - 1, bow);
    cx.lineTo(w / 2 - this.barWidth / 2 - 1, h - bow);
    cx.moveTo(w - sbw - bow, bow);
    cx.lineTo(w - sbw - bow, h - bow);

    const wh = this.whiteHome;
    cx.moveTo(wh.x + wh.width - hbw, wh.y + hbw);
    cx.lineTo(wh.x + wh.width - hbw, wh.y + wh.height - hbw - 1);
    cx.lineTo(wh.x + hbw, wh.y + wh.height - hbw - 1);

    const bh = this.blackHome;
    cx.moveTo(bh.x + bh.width - hbw, bh.y + hbw);
    cx.lineTo(bh.x + bh.width - hbw, bh.y + bh.height - hbw - 1);
    cx.lineTo(bh.x + hbw, bh.y + bh.height - hbw - 1);
    cx.moveTo(bh.x + hbw, bh.y - hbw);
    cx.lineTo(bh.x + bh.width + hbw, bh.y - hbw);

    cx.stroke();

    // dark 3d effect
    cx.beginPath();
    cx.strokeStyle = !this.flipped ? '#222' : '#444';
    cx.lineWidth = 2;
    cx.moveTo(sbw + bow, h - bow);
    cx.lineTo(sbw + bow, bow);

    cx.lineTo(w - sbw - bow, bow);
    cx.moveTo(w / 2 + this.barWidth / 2 + 1, bow);
    cx.lineTo(w / 2 + this.barWidth / 2 + 1, h - bow);
    cx.moveTo(sbw, h - 1);
    const rightEdge = w - sbw / 2 + bow + 1;
    cx.lineTo(rightEdge, h - 1);
    cx.moveTo(rightEdge, 1); // top right
    cx.lineTo(rightEdge, wh.height + bow);
    cx.lineTo(w - sbw, wh.height + bow);
    cx.lineTo(w - sbw, bh.y - hbw);
    cx.moveTo(rightEdge, bh.y - hbw);
    cx.lineTo(rightEdge, h);

    cx.moveTo(wh.x + wh.width - hbw, wh.y + hbw);
    cx.lineTo(wh.x + hbw, wh.y + hbw);
    cx.lineTo(wh.x + hbw, wh.y + wh.height - hbw - 1);

    cx.moveTo(bh.x + bh.width - hbw, bh.y + hbw);
    cx.lineTo(bh.x + hbw, bh.y + hbw);
    cx.lineTo(bh.x + hbw, bh.y + bh.height - hbw - 1);

    cx.stroke();
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
        this.dragging = new CheckerDrag(
          rect,
          clientX,
          clientY,
          ptIdx,
          move.color
        );
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
      this.requestDraw();
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
    this.requestDraw();
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
    this.requestDraw();
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
