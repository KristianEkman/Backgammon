import {
  ChangeDetectionStrategy,
  EventEmitter,
  HostListener,
  OnChanges,
  Output,
  SimpleChanges,
  ViewChild
} from '@angular/core';
import { AfterViewInit, Component, ElementRef, Input } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Subscription } from 'rxjs';
import { MoveDto, GameDto, PlayerColor, GameState } from 'src/app/dto';
import { AppStateService } from 'src/app/state/app-state.service';
import { CheckerArea, CheckerDrag, Point, MoveAnimation } from './';
import { Checker } from './checker';
import {
  BlueTheme,
  DarkTheme,
  GreenTheme,
  IThemes,
  LightTheme,
  PinkTheme
} from './themes';
declare var $: any;

@Component({
  selector: 'app-game-board',
  templateUrl: './game-board.component.html',
  styleUrls: ['./game-board.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GameBoardComponent implements AfterViewInit, OnChanges {
  @ViewChild('canvas') public canvas: ElementRef | undefined;

  @Input() public width = 600;
  @Input() public height = 400;
  @Input() game: GameDto | null = null;
  @Input() myColor: PlayerColor | null = PlayerColor.black;
  @Input() dicesVisible: boolean | null = false;
  @Input() rotated = false;
  @Input() flipped = false;
  @Input() themeName: string | null = 'dark';
  @Input() timeLeft: number | null = 0;
  @Input() tutorialStep: number | null = 0;
  @Input() editing: boolean = false;

  @Output() addMove = new EventEmitter<MoveDto>();
  @Output() addEditMove = new EventEmitter<MoveDto>();
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

  // translated phrases
  you = '';
  white = '';
  black = '';
  left = '';

  constructor(
    private translateService: TranslateService,
    private appState: AppStateService
  ) {
    for (let r = 0; r < 26; r++) {
      this.checkerAreas.push(new CheckerArea(0, 0, 0, 0, 0));
    }

    this.animationSubscription = this.appState.moveAnimations
      .observe()
      .subscribe((moves: MoveDto[]) => {
        if (moves.length > 0 && this.animatedMove === undefined) {
          // console.log('starting animation ');
          this.animatedMove = new MoveAnimation(
            moves[0],
            this.getAnimationPoint(moves[0], moves[0].from, true),
            this.getAnimationPoint(moves[0], moves[0].to, false),
            this.theme,
            this.rotated || this.flipped,
            () => {
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
    if (this.cx) this.cx.imageSmoothingEnabled = true;

    this.translate();
    this.requestDraw();

    this.translateService.onLangChange.subscribe(() => {
      this.translate();
    });
  }

  private _theme: IThemes | undefined = undefined;
  get theme(): IThemes {
    if (!!this._theme && this._theme.name === this.themeName)
      return this._theme;

    this._theme = new DarkTheme();
    if (this.themeName === 'light') this._theme = new LightTheme();
    if (this.themeName === 'blue') this._theme = new BlueTheme();
    if (this.themeName === 'pink') this._theme = new PinkTheme();
    if (this.themeName === 'green') this._theme = new GreenTheme();

    return this._theme as IThemes;
  }

  translate(): void {
    this.you = this.translateService.instant('gameboard.you');
    this.white = this.translateService.instant('gameboard.white');
    this.black = this.translateService.instant('gameboard.black');
    this.left = this.translateService.instant('gameboard.left');
    this.requestDraw();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (
      changes['width'] ||
      changes['height'] ||
      changes['flipped'] ||
      changes['rotated']
    ) {
      this.recalculateGeometry();
    }
    this.requestDraw();
    const bName =
      this.myColor === PlayerColor.black
        ? this.you
        : this.game?.blackPlayer.name;
    const wName =
      this.myColor === PlayerColor.white
        ? this.you
        : this.game?.whitePlayer.name;
    const bLeft = this.game?.blackPlayer.pointsLeft;
    const wLeft = this.game?.whitePlayer.pointsLeft;
    this.blacksName = this.game ? `${bName} - ${bLeft} ${this.left}` : '';
    this.whitesName = this.game ? `${wName} - ${wLeft} ${this.left}` : '';
    // console.log(this.game?.playState);
  }

  @HostListener('window:orientationchange', ['$event'])
  onOrientationChange(): void {
    this.recalculateGeometry();
    // console.log('orient change');
  }

  recalculateGeometry(): void {
    if (this.canvas) {
      const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
      canvasEl.width = this.width;
      canvasEl.height = this.height;
    }

    this.borderWidth = this.width * 0.01;
    this.barWidth = this.borderWidth * 3;
    this.sideBoardWidth = this.width * 0.1;

    const rectH = this.height * 0.42;
    const rectBase = this.getRectBase();

    //blacks bar
    this.checkerAreas[24].set(
      this.width / 2 - rectBase / 2,
      rectH / 3,
      rectBase,
      rectH / 1.5 + this.borderWidth,
      0
    );
    this.blackBar = this.checkerAreas[24];

    //whites bar
    this.checkerAreas[25].set(
      this.width / 2 - rectBase / 2,
      this.height / 2 + this.height * 0.08 - this.borderWidth,
      rectBase,
      rectH / 1.5,
      25
    );

    this.whiteBar = this.checkerAreas[25];

    //blacks home
    this.blackHome.set(
      this.width - this.sideBoardWidth - this.borderWidth / 2,
      this.height - this.height * 0.4 - this.borderWidth / 2,
      this.getCheckerWidth() * 2 + this.borderWidth + 2,
      this.height * 0.4,
      25
    );

    //white home
    this.whiteHome.set(
      this.width - this.sideBoardWidth - this.borderWidth / 2,
      this.borderWidth / 2,
      this.getCheckerWidth() * 2 + this.borderWidth + 2,
      this.height * 0.4,
      0
    );

    let x = this.borderWidth + this.sideBoardWidth;
    let y = this.borderWidth;

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
    this.drawBoard(cx);
    this.drawCheckers(cx);
    if (this.animatedMove) {
      this.animatedMove.draw(cx, this.getCheckerWidth());
    }

    this.drawClock(cx);

    if (<number>this.tutorialStep > 0) {
      this.drawTutorial(cx);
    }

    // *** NOT PROD CODE
    // this.drawIcon(cx);
    // this.drawDebugRects(cx);
    // this.drawTrace(cx);
    // *** NOT PROD CODE
    return 0;
  }

  // drawTrace(cx: CanvasRenderingContext2D) {
  //   cx.strokeStyle = 'red';
  //   if (this.trace.length < 1) return;
  //   cx.moveTo(this.trace[0].x, this.trace[0].y);
  //   this.trace.forEach((element) => {
  //     cx.lineTo(element.x, element.y);
  //   });
  //   cx.stroke();
  // }

  drawTutorial(cx: CanvasRenderingContext2D) {
    cx.strokeStyle = this.theme.highLight;
    cx.lineWidth = 2;
    cx.lineJoin = 'bevel';

    switch (this.tutorialStep) {
      case 2: {
        this.drawYourDirection(cx);
        break;
      }
      case 3: {
        this.drawOpponentDirection(cx);
        break;
      }
      case 4: {
        this.drawHomeArrow(cx);
        break;
      }
      default:
        break;
    }
  }

  drawYourDirection(cx: CanvasRenderingContext2D) {
    const chWidth = this.getCheckerWidth();
    const arrowSize = chWidth / 3;
    // First arrow from pos 1
    var area = this.checkerAreas.filter((r) => r.pointIdx === 1)[0];
    cx.beginPath();
    var x = area.x + area.width / 2;
    var y = area.y + chWidth;
    cx.moveTo(x, y);
    x -= area.width * 2;
    cx.lineTo(x, y);
    cx.lineTo(x + arrowSize, y + arrowSize);
    cx.moveTo(x + arrowSize, y - arrowSize);
    cx.lineTo(x, y); // -1.5 is a hack. Must be a way to make arrows look better.
    cx.stroke();

    // second arrow from pos 12
    area = this.checkerAreas.filter((r) => r.pointIdx === 12)[0];
    x = area.x + area.width / 2;
    y = area.y + area.height - chWidth * 2;
    cx.moveTo(x, y);
    y += chWidth * 4;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y - arrowSize);
    cx.moveTo(x, y);
    cx.lineTo(x + arrowSize, y - arrowSize);
    cx.stroke();

    //3rd line from pos 17
    area = this.checkerAreas.filter((r) => r.pointIdx === 17)[0];
    x = area.x + area.width / 2;
    y = area.y + area.height - chWidth;
    cx.moveTo(x, y);
    x += area.width * 2;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y - arrowSize);
    cx.moveTo(x, y);
    cx.lineTo(x - arrowSize, y + arrowSize);

    cx.stroke();

    //4th line from pos 19
    area = this.checkerAreas.filter((r) => r.pointIdx === 19)[0];
    x = area.x + area.width / 2;
    y = area.y + area.height - chWidth;
    cx.moveTo(x, y);
    x += area.width * 6;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y - arrowSize);
    cx.moveTo(x, y);
    cx.lineTo(x - arrowSize, y + arrowSize);

    cx.stroke();
  }

  drawHomeArrow(cx: CanvasRenderingContext2D) {
    const chWidth = this.getCheckerWidth();
    const arrowSize = chWidth / 3;
    var area = this.checkerAreas.filter((r) => r.pointIdx === 22)[0];
    cx.beginPath();
    var x = area.x + area.width / 2;
    var y = area.y + area.height - chWidth;
    cx.moveTo(x, y);
    x += area.width * 3;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y + arrowSize);
    cx.moveTo(x - arrowSize, y - arrowSize);
    cx.lineTo(x, y); // -1.5 is a hack. Must be a way to make arrows look better.
    cx.stroke();

    var area1 = this.checkerAreas.filter((r) => r.pointIdx === 19)[0];
    cx.setLineDash([3, 3]);
    cx.strokeRect(area1.x, area1.y, area1.width * 6, area1.height);
    cx.setLineDash([]);
  }

  drawOpponentDirection(cx: CanvasRenderingContext2D) {
    const chWidth = this.getCheckerWidth();
    const arrowSize = chWidth / 3;
    // First arrow from pos 24
    var area = this.checkerAreas.filter((r) => r.pointIdx === 24)[0];
    cx.beginPath();
    var x = area.x + area.width / 2;
    var y = area.y + area.height - chWidth;
    cx.moveTo(x, y);
    x -= area.width * 2;
    cx.lineTo(x, y);
    cx.lineTo(x + arrowSize, y + arrowSize);
    cx.moveTo(x + arrowSize, y - arrowSize);
    cx.lineTo(x, y); // -1.5 is a hack. Must be a way to make arrows look better.
    cx.stroke();

    // second arrow from pos 13
    area = this.checkerAreas.filter((r) => r.pointIdx === 13)[0];
    x = area.x + area.width / 2;
    y = area.y + chWidth * 2;
    cx.moveTo(x, y);
    y -= chWidth * 4;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y + arrowSize);
    cx.moveTo(x, y);
    cx.lineTo(x + arrowSize, y + arrowSize);
    cx.stroke();

    //3rd line from pos 8
    area = this.checkerAreas.filter((r) => r.pointIdx === 8)[0];
    x = area.x + area.width / 2;
    y = area.y + chWidth;
    cx.moveTo(x, y);
    x += area.width * 2;
    cx.lineTo(x, y);
    cx.lineTo(x - arrowSize, y - arrowSize);
    cx.moveTo(x, y);
    cx.lineTo(x - arrowSize, y + arrowSize);
    cx.stroke();

    cx.stroke();
  }

  drawClock(cx: CanvasRenderingContext2D) {
    // clock
    if (!this.timeLeft || this.timeLeft < 0) return;

    cx.beginPath();
    const x = this.whiteHome.x + this.whiteHome.width / 2 + this.borderWidth;
    const y = this.height / 2;
    const a = ((this.timeLeft ?? 0) / 40) * 2;
    const s = this.height * 0.06;
    cx.fillStyle = 'green';
    cx.moveTo(x, y);
    if (this.rotated) {
      cx.save();
      cx.translate(x * 2, y * 2);
      cx.rotate(Math.PI);
    }

    if (this.flipped) {
      cx.save();
      cx.translate(0, y * 2);
      cx.scale(1, -1);
    }

    cx.arc(x, y, s, -(a + 0.5) * Math.PI, -0.5 * Math.PI);
    cx.lineTo(x, y);
    cx.shadowBlur = 0;
    cx.shadowOffsetX = 0;
    cx.shadowOffsetY = 0;
    cx.fillStyle = this.theme.boardBackground;
    if (this.timeLeft < 20) {
      const h = Math.round((this.timeLeft / 40) * 128);
      cx.fillStyle = `hsl(${h}, 20%, 50%)`;
    }
    cx.fill();

    cx.fillStyle = this.theme.textColor;
    if (this.timeLeft < 20) {
      const h = Math.round((this.timeLeft / 40) * 128);
      cx.fillStyle = `hsl(${h}, 100%, 50%)`;
    }
    cx.font = `bold ${Math.ceil(s)}px Arial`;
    cx.textAlign = 'center';

    const tx = x;
    const ty = y + s / 2 - s / 6;
    const text = Math.ceil(this.timeLeft).toString();

    cx.fillText(text, tx, ty);
    cx.strokeStyle = this.theme.textColor;
    if (this.rotated || this.flipped) {
      cx.restore();
    }
    // cx.strokeText(text, tx, ty);
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

  getAnimationPoint(moveDto: MoveDto, pNum: number, start: boolean): Point {
    if (!this.game) {
      return new Point(0, 0);
    }
    let pointIdx = 0;

    if (moveDto.color === PlayerColor.black) {
      if (pNum === 0) {
        return this.blackBar.getCenter();
      }
      if (pNum === 25) {
        return this.blackHome.getCenter();
        // black bar
      }
    } else {
      if (pNum === 0) {
        return this.whiteBar.getCenter();
      }
      if (pNum === 25) {
        return this.whiteHome.getCenter();
      }
    }

    if (moveDto.color === PlayerColor.black) {
      pointIdx = pNum;
    } else {
      // white move
      pointIdx = 25 - pNum;
    }

    const area = this.checkerAreas.find((r) => r.pointIdx === pointIdx);
    let checkCount = this.game.points[pointIdx].checkers.length;
    if (start) {
      checkCount--;
    }

    const cw = this.getCheckerWidth();
    if (!area) return new Point(0, 0);

    // distance between checkers
    const dist = Math.min(2 * cw, (area.height - cw) / checkCount);
    let y = area.y + checkCount * dist + dist / 2;
    if (pointIdx > 12) {
      y = area.y + area.height - checkCount * dist - dist / 2;
    }
    const x = area.x + area.width / 2;
    return new Point(x, y);
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

    const r = this.getCheckerRadius();
    const chWidth = this.getCheckerWidth();

    for (let p = 1; p < 25; p++) {
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

      //Preventing animated move to be drawn at its destination until animation is finished.
      const dragAnimationTo =
        this.animatedMove &&
        !this.animatedMove.move.hint &&
        ((this.animatedMove?.move.color === PlayerColor.black &&
          this.animatedMove.move.to == p) ||
          (this.animatedMove?.move.color === PlayerColor.white &&
            this.animatedMove.move.to == 25 - p));

      if (dragAnimationTo) {
        checkerCount--;
      }

      // distance between checkers on a point
      const dist = Math.min(
        2 * chWidth,
        (area.height - chWidth) / checkerCount
      );

      // main draw checkers loop
      for (let i = 0; i < checkerCount; i++) {
        const checker = point.checkers[i];
        const x = area.x + r;
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
          this.rotated || this.flipped,
          false
        );
      }
    }

    this.drawHomes(cx);
    this.drawBars(cx);

    if (this.dragging) {
      Checker.draw(
        cx,
        this.cursor,
        chWidth,
        this.theme,
        this.dragging.color,
        false,
        true,
        this.rotated || this.flipped,
        false
      );
    }
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
    const fntSize = Math.round(this.blackHome.width / 3);
    const homeCountFntSize = 'bold ' + fntSize + 'px Arial';
    cx.font = homeCountFntSize;
    const wi = this.sideBoardWidth / 2 - bw;
    for (let i = 0; i < blackCount; i++) {
      cx.fillRect(x, y - i * (chb + 1), wi, chb);
    }

    if (blackCount > 0) {
      if (this.rotated) {
        cx.save();
        cx.translate(x, y - blackCount * (chb + 1) - fntSize / 2);
        cx.rotate(Math.PI);
        cx.fillText(`${blackCount}`, -wi, 0);
        cx.restore();
      } else if (this.flipped) {
        cx.save();
        cx.translate(
          x + this.blackHome.width,
          y - blackCount * (chb + 1) - fntSize / 2
        );
        cx.scale(1, -1);
        cx.fillText(`${blackCount}`, -wi, 0);
        cx.restore();
      } else {
        cx.fillText(`${blackCount}`, x, y - blackCount * (chb + 1) + 3);
      }
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

    if (whiteCount > 0) {
      if (this.rotated) {
        cx.save();
        cx.translate(x, y + whiteCount * (chw + 1));
        cx.rotate(Math.PI);
        cx.fillText(`${whiteCount}`, -wi, 0);
        cx.restore();
      } else if (this.flipped) {
        cx.save();
        cx.translate(x + this.whiteHome.width, y + whiteCount * (chw + 1));
        cx.scale(1, -1);
        cx.fillText(`${whiteCount}`, -wi, 0);
        cx.restore();
      } else {
        cx.fillText(`${whiteCount}`, x, y + whiteCount * (chw + 1) + fntSize);
      }
    }

    //draw homes if can be moved to
    if (this.blackHome.canBeMovedTo) {
      this.blackHome.highLightBottom(cx);
    }

    if (this.whiteHome.canBeMovedTo) {
      this.whiteHome.highLightTop(cx);
    }
  }

  drawBars(cx: CanvasRenderingContext2D): void {
    // draw checkers at  home.
    if (!this.game) {
      return;
    }
    // black
    let blackCount = this.game.points[0].checkers.filter(
      (c) => c.color === PlayerColor.black
    ).length;
    const draggingBlack =
      this.dragging && this.dragging.checkerArea == this.blackBar;
    if (
      (this.animatedMove &&
        this.animatedMove?.move.color === PlayerColor.black &&
        this.animatedMove.move.to == 0) ||
      draggingBlack
    ) {
      blackCount--;
    }

    const bw = this.borderWidth;
    let x = this.blackBar.x + this.blackBar.width / 2;
    let y = this.blackBar.y + this.blackBar.height - bw * 1.5;
    const chWidth = this.getCheckerWidth();

    const highLightBlack = this.blackBar.hasValidMove && !draggingBlack;

    for (let i = 0; i < blackCount; i++) {
      const yi = y - i * chWidth * 1.5;
      Checker.draw(
        cx,
        { x, y: yi },
        chWidth,
        this.theme,
        PlayerColor.black,
        highLightBlack && i === blackCount - 1,
        false,
        this.rotated || this.flipped,
        false
      );
    }

    // white
    let whiteCount = this.game.points[25].checkers.filter(
      (c) => c.color === PlayerColor.white
    ).length;
    const draggingWhite =
      this.dragging && this.dragging.checkerArea == this.whiteBar;
    if (
      (this.animatedMove &&
        this.animatedMove?.move.color === PlayerColor.white &&
        this.animatedMove.move.to == 0) ||
      draggingWhite
    ) {
      whiteCount--;
    }

    x = this.whiteBar.x + this.whiteBar.width / 2;
    y = this.whiteBar.y + bw;

    const highLightWhite = this.whiteBar.hasValidMove && !draggingWhite;

    for (let i = 0; i < whiteCount; i++) {
      const yi = y + i * chWidth * 1.5;
      Checker.draw(
        cx,
        { x, y: yi },
        chWidth,
        this.theme,
        PlayerColor.white,
        highLightWhite && i === whiteCount - 1,
        false,
        this.rotated || this.flipped,
        false
      );
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
    this.whiteHome.fill(cx, this.theme.homeBackground);
    this.whiteHome.drawBorder(cx, false);
    this.blackHome.fill(cx, this.theme.homeBackground);
    this.blackHome.drawBorder(cx, false);
    const homeFntSize = this.blackHome.width / 2 + 'px Arial';
    // names of players
    cx.font = '18px Arial';
    cx.fillStyle = this.theme.textColor;
    cx.save();
    cx.translate(this.blackHome.x, this.blackHome.y);
    cx.rotate(Math.PI / 2);
    if (this.flipped) {
      cx.scale(-1, 1);
      cx.translate(-this.blackHome.height, 0);
    }

    cx.fillStyle = this.theme.textColor;
    cx.fillText(this.blacksName, 0, -this.blackHome.width - 11);
    cx.fillStyle = this.theme.border;
    cx.font = homeFntSize;
    cx.fillText(
      this.black,
      this.borderWidth / 2 + 2,
      -this.borderWidth / 2 - 2,
      this.blackHome.height
    );
    cx.restore();

    cx.save();
    cx.translate(this.whiteHome.x, this.whiteHome.y);
    cx.rotate(Math.PI / 2);
    if (this.flipped) {
      cx.scale(-1, 1);
      cx.translate(-this.whiteHome.height, 0);
    }
    cx.font = '18px Arial';
    cx.fillStyle = this.theme.textColor;
    cx.fillText(this.whitesName, 0, -this.whiteHome.width - 11);
    cx.fillStyle = this.theme.border;
    cx.font = homeFntSize;

    cx.fillText(
      this.white,
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
    cx.strokeStyle = !this.rotated ? '#444' : '#222';
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
    cx.strokeStyle = !this.rotated ? '#222' : '#444';
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
    return {
      x: event.offsetX + this.borderWidth,
      y: event.offsetY + this.borderWidth
    };
  }

  getTouchPoint(touch: any): Point {
    const eventX = touch.pageX || touch.originalEvent?.pageX;
    const eventY = touch.pageY || touch.originalEvent?.pageY;
    const offset = $('canvas').offset();

    if (this.rotated) {
      return {
        x: this.width - eventX + offset.left,
        y: this.height - eventY + offset.top
      };
    }

    if (this.flipped) {
      return {
        x: eventX - offset.left,
        y: this.height - eventY + offset.top
      };
    }

    return {
      x: eventX - offset.left,
      y: eventY - offset.top
    };
  }

  handleDown(clientX: number, clientY: number): void {
    if (!this.game) {
      return;
    }

    if (!this.canMove()) {
      return;
    }

    const areas = this.checkerAreas;
    if (this.editing) {
      areas.push(this.blackHome);
      areas.push(this.whiteHome);
    }

    for (let i = 0; i < areas.length; i++) {
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
      const checkers = this.game.points[ptIdx].checkers;
      if (move !== undefined || (this.editing && checkers.length > 0)) {
        const lastChecker = checkers[checkers.length - 1];
        this.dragging = new CheckerDrag(
          rect,
          clientX,
          clientY,
          ptIdx,
          lastChecker.color
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

    if (this.editing) return true;
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

    if (this.editing) {
      this.doEditMove(clientX, clientY);
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
      Math.abs(clientX - xDown) < 20 && Math.abs(clientY - yDown) < 20;

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

  doEditMove(clientX: number, clientY: number): void {
    if (!this.dragging) return;

    if (!this.game) return;

    const { xDown, yDown, fromIdx, color } = this.dragging;

    const allRects: CheckerArea[] = [...this.checkerAreas];
    if (color === PlayerColor.black) {
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

      let move: MoveDto = {
        color: this.dragging.color,
        from: fromIdx,
        nextMoves: [],
        to: ptIdx,
        animate: false,
        hint: false
      };

      this.addEditMove.emit({ ...move });
      break;
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

  onTouchEnd(event: TouchEvent): void {
    // console.log('touchend', event);
    // console.log(this.cursor);

    if (this.cursor != undefined) {
      this.handleUp(this.cursor.x, this.cursor.y);
    }
    this.lastTouch = undefined;
  }

  lastTouch: Point | undefined = undefined;

  // trace: Point[] = [];
  onTouchMove(event: TouchEvent): void {
    // console.log('touchmove');
    if (event.touches.length !== 1) {
      return;
    }
    const touch = event.touches[0];
    const { x, y } = this.getTouchPoint(touch);
    // this.trace.push({ x: x, y: y });

    this.lastTouch = { x, y };
    this.handleMove(x, y);
  }
}
