import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ChatService } from 'src/app/services/chat.service';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-chat-container',
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent {
  constructor(private stateService: AppStateService, private fb : FormBuilder, private chatService: ChatService) {
    
    this.formGroup = this.fb.group({
      message: ['']
    })    
  }

  formGroup : FormGroup;

  @ViewChild('msginput') input!: ElementRef;

  open = this.stateService.chatOpen.observe();
  onClickOpen() {
    const open = this.stateService.chatOpen.getValue();
    this.stateService.chatOpen.setValue(!open);
    this.chatService.connect();

    if (!open) {
      setTimeout(() => {
        this.input.nativeElement.focus();
      }, 1);
    }
  }

  onClickClose() {
    this.stateService.chatOpen.setValue(false);
  }

  onSubmit() {
    const ctrl = this.formGroup.get("message");
    const message = ctrl?.value;
    ctrl?.setValue(''); 
    console.log(message);
    this.chatService.sendMessage(message);
  }
}
