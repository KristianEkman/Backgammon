import { Component, EventEmitter, Output } from '@angular/core';
import { MessageType } from 'src/app/dto';

@Component({
  selector: 'app-mailing',
  templateUrl: './mailing.component.html',
  styleUrls: ['./mailing.component.scss']
})
export class MailingComponent {
  @Output() onSend = new EventEmitter<MessageType>();

  sendInfo(): void {
    this.onSend.emit(MessageType.version2Info);
  }
}
