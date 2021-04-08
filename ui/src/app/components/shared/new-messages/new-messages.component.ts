import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MessageDto } from 'src/app/dto';

@Component({
  selector: 'app-new-messages',
  templateUrl: './new-messages.component.html',
  styleUrls: ['./new-messages.component.scss']
})
export class NewMessagesComponent implements OnInit {
  @Input() messages: MessageDto[] | null = null;
  @Output() showMessages = new EventEmitter<void>();

  ngOnInit(): void {}

  buttonClick(): void {
    this.showMessages.emit();
  }
}
