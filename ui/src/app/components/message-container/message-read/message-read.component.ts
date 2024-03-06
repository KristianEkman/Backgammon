import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MessageDto, MessageType } from 'src/app/dto';
import { MessageTypeText } from './messageTypeTexts';
import { SharePromptComponent } from '../share-prompt/share-prompt.component';
import { TranslateModule } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-message-read',
  standalone: true,
  templateUrl: './message-read.component.html',
  styleUrls: ['./message-read.component.scss'],
  // changeDetection: ChangeDetectionStrategy.OnPush
  imports: [SharePromptComponent, TranslateModule, CommonModule]
})
export class MessageReadComponent {
  @Input() messages: MessageDto[] | null = null;
  @Output() delete = new EventEmitter<number>();
  MessageType = MessageType;
  currentIndex = 0;
  hide = false;
  deleteAnim = false;

  constructor() {}

  deleteClick(): void {
    if (this.messages) {
      this.animateDelete();
      this.delete.emit(this.messages[this.currentIndex].id);
      this.currentIndex = 0;
      // little bit ugly to jump first here, but will do for now.
    }
  }

  animateDelete(): void {
    this.deleteAnim = true;
    setTimeout(() => {
      this.deleteAnim = false;
    }, 600);
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
