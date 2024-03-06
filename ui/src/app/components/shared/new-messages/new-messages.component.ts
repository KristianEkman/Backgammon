import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { MessageDto } from 'src/app/dto';

@Component({
  selector: 'app-new-messages',
  standalone: true,
  templateUrl: './new-messages.component.html',
  styleUrls: ['./new-messages.component.scss'],
  imports: [CommonModule]
})
export class NewMessagesComponent {
  @Input() messages: MessageDto[] | null = null;
  @Output() showMessages = new EventEmitter<void>();

  constructor(private trans: TranslateService) {}

  buttonClick(): void {
    this.showMessages.emit();
  }

  buttonText(): string {
    if (!this.messages) return '';

    if (this.messages.length < 2) {
      return this.trans.instant('newmessages.one', {
        count: this.messages.length
      });
    }

    return this.trans.instant('newmessages.many', {
      count: this.messages.length
    });
  }
}
