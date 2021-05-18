import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MessageDto } from 'src/app/dto';
import { MessageService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-message-container',
  templateUrl: './message-container.component.html',
  styleUrls: ['./message-container.component.scss']
})
export class MessageContainerComponent implements OnInit {
  constructor(private service: MessageService) {
    this.messages$ = AppState.Singleton.messages.observe();
  }

  messages$: Observable<MessageDto[]>;
  ngOnInit(): void {
    this.service.loadMessages();
  }

  deleteMessage(id: number): void {
    this.service.deleteMessage(id);
  }
}
