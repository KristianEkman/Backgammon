import {
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges
} from '@angular/core';
import {
  trigger,
  state,
  style,
  animate,
  transition
} from '@angular/animations';
import { StatusMessage, MessageLevel } from '../../../dto/local/status-message';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.scss'],
  animations: [
    trigger('showHide', [
      state(
        'initial',
        style({
          left: '{{ shown }}px',
          transform: 'scale(3)',
          opacity: 0.3,
          top: 100
        }),
        { params: { shown: 0 } }
      ),
      state(
        'shown',
        style({
          left: '{{ shown }}px',
          transform: 'scale(1)',
          opacity: 1,
          top: 0
        }),
        { params: { shown: 0 } }
      ),
      state(
        'hidden',
        style({
          left: '0px',
          opacity: 0,
          transform: 'scale(1)'
        })
      ),
      transition('shown => hidden', [animate('0.5s ease-out')]),
      transition('hidden => initial', [animate('0.01s')]),
      transition('initial => shown', [animate('2s ease')])
    ])
  ]
})
export class MessagesComponent implements OnChanges, OnInit {
  @Input() message: StatusMessage | null = StatusMessage.getDefault();

  // changing the coordinates will affect all animations coordinates.
  @Input() initial = 0;

  //x coordinate when shown.
  @Input() shown = 0;
  state = 'hidden';
  animating = false;
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['message']) {
      this.animate();
    }
  }

  ngOnInit(): void {
    // this.animate();
  }

  animate(): void {
    if (this.animating) return;
    this.animating = true;

    this.state = 'hidden';
    setTimeout(() => {
      this.state = 'initial';
      setTimeout(() => {
        this.state = 'shown';
        this.animating = false;
      }, 100);
    }, 500);
  }

  getIcon(): string {
    if (this.message?.level === MessageLevel.error) {
      return 'fas fa-exclamation-circle error-color';
    }

    if (this.message?.level === MessageLevel.warning) {
      return 'fas fa-exclamation-triangle yellow';
    }

    return '';
  }
}
