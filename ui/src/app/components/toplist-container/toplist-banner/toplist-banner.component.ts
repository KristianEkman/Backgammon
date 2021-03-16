import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  ViewChild
} from '@angular/core';
import {
  trigger,
  state,
  style,
  animate,
  transition
} from '@angular/animations';
import { Toplist, ToplistResult } from 'src/app/dto';

@Component({
  selector: 'app-toplist-banner',
  templateUrl: './toplist-banner.component.html',
  styleUrls: ['./toplist-banner.component.scss'],
  animations: [
    trigger('scrollBanner', [
      state(
        'initial',
        style({
          left: '{{ initial }}px'
        }),
        { params: { initial: 0 } }
      ),
      state(
        'shown',
        style({
          left: '{{ shown }}px'
        }),
        { params: { shown: 0 } }
      ),
      transition('initial => shown', [animate('20s')]),
      transition('show => initial', [animate('0.1s')])
    ])
  ]
})
export class ToplistBannerComponent implements OnChanges {
  @Input() toplist: Toplist | null = null;
  @Output() clicked = new EventEmitter<void>();
  @ViewChild('banner') bannerRef: ElementRef | null = null;
  @ViewChild('bannerItems') bannerItemsRef: ElementRef | null = null;

  reversedList: ToplistResult[] = [];
  initial = 340;
  shown = 0;
  rollingState = '';
  timeoutHandle: any = null;
  started = false;

  ngOnChanges(): void {
    if (this.toplist) {
      const temp = [...this.toplist.results];
      this.reversedList = temp.reverse();

      this.timeoutHandle = setTimeout(() => {
        if (this.bannerRef) {
          this.initial = (this.bannerRef
            .nativeElement as HTMLElement).clientWidth;
        }

        if (this.bannerItemsRef) {
          this.shown = -(this.bannerItemsRef.nativeElement as HTMLElement)
            .clientWidth;
        }

        this.started = true;
        this.rollingState = 'shown';
        clearTimeout(this.timeoutHandle);
      }, 2000);
    }
  }

  scrollFinished(): void {
    if (!this.started) {
      return;
    }
    if (this.rollingState === 'initial') {
      this.rollingState = 'shown';
    } else {
      this.rollingState = 'initial';
    }
  }

  bannerClick(): void {
    this.clicked.emit();
  }
}