import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output
} from '@angular/core';
import { MessageDto, MessageType } from 'src/app/dto';
import { MessageTypeText } from './messageTypeTexts';

@Component({
  selector: 'app-message-read',
  templateUrl: './message-read.component.html',
  styleUrls: ['./message-read.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MessageReadComponent implements OnInit {
  @Input() messages: MessageDto[] | null = null;
  @Output() delete = new EventEmitter<number>();
  MessageType = MessageType;
  currentIndex = 0;
  hide = false;

  ngOnInit(): void {}

  deleteClick(): void {
    if (this.messages) {
      this.animateHide();
      this.delete.emit(this.messages[this.currentIndex].id);
      this.currentIndex = 0;
      // little bit ugly to jump first here, but will do for now.
    }
  }

  animateHide(): void {
    this.hide = true;
    setTimeout(() => {
      this.hide = false;
    }, 600);
  }

  get message(): MessageDto | null {
    if (this.messages) {
      return this.messages[this.currentIndex];
    }
    return null;
  }

  nextClick(): void {
    this.animateHide();

    this.currentIndex++;
  }

  getTypeText(type: MessageType): string {
    return MessageTypeText.find((m) => m.type === type)?.message ?? '';
  }
}
