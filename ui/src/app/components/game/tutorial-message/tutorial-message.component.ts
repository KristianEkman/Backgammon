import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-tutorial-message',
  templateUrl: './tutorial-message.component.html',
  styleUrls: ['./tutorial-message.component.scss']
})
export class TutorialMessageComponent implements AfterViewInit {
  ngAfterViewInit(): void {
    setTimeout(() => {
      this.showMe();
    }, 500);
  }

  @Input() step: number | null = 0;
  @Output() next = new EventEmitter<void>();
  @Output() back = new EventEmitter<void>();

  show = false;
  hide = true;

  showMe() {
    this.show = true;
    this.hide = false;
  }

  hideMe() {
    this.show = false;
    this.hide = true;
  }

  nextButtonClick() {
    this.hideMe();
    setTimeout(() => {
      this.showMe();
    }, 200);
    this.next.emit();
  }

  backButtonClick() {
    this.hideMe();
    setTimeout(() => {
      this.showMe();
    }, 200);
    this.back.emit();
  }

  get backVisible(): boolean {
    return <boolean>(this.step && this.step > 1);
  }
}
