import {
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Observable, map } from 'rxjs';
import { ChatService } from 'src/app/services/chat.service';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-chat-container',
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent implements OnDestroy {
  constructor(
    private stateService: AppStateService,
    private fb: FormBuilder,
    private chatService: ChatService
  ) {
    this.formGroup = this.fb.group({
      message: ['']
    });
  }

  map = map;
  formGroup: FormGroup;

  @ViewChild('msginput') input!: ElementRef;

  open$ = this.stateService.chatOpen.observe();
  chatMessages$ = this.stateService.chatMessages.observe().pipe(
    map((m) => m.map((n) => `${n.fromUser}:\n  ${n.message}`)),
    map((m) => m.join('\n'))
  );

  users$ = this.stateService.chatUsers.observe().pipe(
    map((u) => u?.map((v) => v)),
    map((u) => u?.join('\n'))
  );

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
    this.stateService.chatMessages.setValue([]);
    this.chatService.disconnect();
  }

  onSubmit() {
    const ctrl = this.formGroup.get('message');
    const message = ctrl?.value;
    if (message?.trim()) {
      ctrl?.setValue('');
      this.chatService.sendMessage(message);
    }
  }

  ngOnDestroy(): void {
    this.chatService.disconnect();
  }

  wasInside = false;

  @HostListener('click')
  clickInside() {
    this.wasInside = true;
  }

  @HostListener('document:click')
  clickout() {
    if (!this.wasInside) {
      this.onClickClose();
    }
    this.wasInside = false;
  }
}
