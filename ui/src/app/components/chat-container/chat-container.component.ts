import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-chat-container',
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent {
  constructor(private stateService: AppStateService) {}

  @ViewChild('msginput') input!: ElementRef;

  open = this.stateService.chatOpen.observe();
  onClickOpen() {
    const open = this.stateService.chatOpen.getValue();
    this.stateService.chatOpen.setValue(!open);
    if (!open) {
      setTimeout(() => {
        console.log('focus');
        this.input.nativeElement.focus();
      }, 1);
    }
  }

  onClickClose() {
    this.stateService.chatOpen.setValue(false);
  }
}
