import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { MessageDto } from 'src/app/dto';
import { MessageService } from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';
import { MessageReadComponent } from './message-read/message-read.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-message-container',
  standalone: true,
  templateUrl: './message-container.component.html',
  styleUrls: ['./message-container.component.scss'],
  imports: [MessageReadComponent, CommonModule]
})
export class MessageContainerComponent {
  constructor(
    private service: MessageService,
    private appState: AppStateService
  ) {
    this.messages$ = this.appState.messages.observe();
    this.service.loadMessages();
  }

  messages$: Observable<MessageDto[]>;

  deleteMessage(id: number): void {
    this.service.deleteMessage(id);
  }
}
