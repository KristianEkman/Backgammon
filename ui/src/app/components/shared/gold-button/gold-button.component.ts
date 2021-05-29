import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output
} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import {
  differenceInHours,
  differenceInMinutes,
  differenceInSeconds
} from 'date-fns';
import { Observable, of } from 'rxjs';
import { UserDto } from 'src/app/dto';

@Component({
  selector: 'app-gold-button',
  templateUrl: './gold-button.component.html',
  styleUrls: ['./gold-button.component.scss']
})
export class GoldButtonComponent implements OnDestroy, OnInit {
  @Input() user: UserDto | null = null;
  @Output() getGold = new EventEmitter<void>();

  buttonGetText: Observable<string>;
  intervalHandle: any;
  constructor(private translate: TranslateService) {
    this.buttonGetText = this.getButtonText();
  }

  ngOnInit(): void {
    this.intervalHandle = setInterval(() => {
      this.buttonGetText = this.getButtonText();
    }, 1000);
  }

  ngOnDestroy(): void {
    clearInterval(this.intervalHandle);
  }

  getButtonText(): Observable<string> {
    if (this.user?.gold === undefined) return of('');
    if (this.user.gold > 50) return of('');
    if (!this.user?.lastFreeGold === undefined) return of('');

    const last = this.user.lastFreeGold * 1000; // total seconds to javascript timestamp
    const nextFill = last + 24 * 3600 * 1000; // ms in 24h
    const now = new Date();

    const diff = nextFill - now.getTime();
    if (diff <= 0) return this.translate.get('goldbutton.getgold');

    const h = differenceInHours(new Date(nextFill), now);
    const m = differenceInMinutes(new Date(nextFill), now) - h * 60;
    const s = differenceInSeconds(new Date(nextFill), now) - h * 3600 - m * 60;
    const sh = this.pad(h, 2);
    const sm = this.pad(m, 2);
    const ss = this.pad(s, 2);

    return of(`${sh}:${sm}:${ss}`);
  }

  getGoldClicked(): void {
    this.getGold.emit();
  }

  pad(num: number, size: number): string {
    let s = num + '';
    while (s.length < size) s = '0' + s;
    return s;
  }
}
