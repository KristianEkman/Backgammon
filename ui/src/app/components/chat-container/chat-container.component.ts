import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  ViewChild
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { bufferTime, filter, map } from 'rxjs';
import { ChatService } from 'src/app/services/chat.service';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-chat-container',
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent {
  constructor(
    private stateService: AppStateService,
    private fb: FormBuilder,
    private chatService: ChatService,
    private changeDetector: ChangeDetectorRef
  ) {
    this.formGroup = this.fb.group({
      message: ['']
    });

    this.formGroup
      .get('message')
      ?.valueChanges.pipe(
        bufferTime(500),
        filter((x) => !!x.length),
        map((x) => x[x.length - 1])
      )
      .subscribe((message) => {
        this.chatService.userIsTyping(message);
      });

    // todo: get this to work with observables
    setInterval(() => {
      this.usersCount = this.stateService.chatUsers.getValue().length;
      this.othersInChat = this.usersCount > 1;
    }, 2000);

    this.chatMessages$.subscribe(() => {
      setTimeout(() => {
        const textarea = document.getElementsByClassName('conversation')[0]!;
        textarea.scrollTop = textarea.scrollHeight;
      }, 1);
    });
  }

  formGroup: FormGroup;

  @ViewChild('msginput') input!: ElementRef;
  @ViewChild('conversation') conversation!: ElementRef;

  open$ = this.stateService.chatOpen.observe();

  // chatMessages$ = this.stateService.chatMessages.observe().pipe(
  //   map((m) => m.map((n) => `${n.fromUser}:\n  ${n.message}`)),
  //   map((m) => m.join('\n'))
  // );

  chatMessages$ = this.stateService.chatMessages.observe().pipe(
    map((messages) => {
      const bld = [];
      for (let i = 0; i < messages.length; i++) {
        const element = messages[i];
        if (i === 0 || messages[i - 1].fromUser !== element.fromUser) {
          bld.push(`\n${element.fromUser}`);
        }
        bld.push(`  ${element.message}`);
      }
      return bld;
    }),
    map((m) => m.join('\n'))
  );

  users$ = this.stateService.chatUsers.observe().pipe(
    map((u) => u?.map((v) => v)),
    map((u) => u?.join('\n'))
  );

  usersCount = 0;
  othersInChat = false;

  onClickOpen() {
    const open = this.stateService.chatOpen.getValue();
    this.stateService.chatOpen.setValue(!open);

    if (!open) {
      setTimeout(() => {
        this.input.nativeElement.focus();
      }, 1);
    }
    this.changeDetector.detectChanges();
  }

  onClickClose() {
    this.stateService.chatOpen.setValue(false);
    this.stateService.chatMessages.setValue([]);
    this.changeDetector.detectChanges();
  }

  onSubmit() {
    const ctrl = this.formGroup.get('message');
    const message = ctrl?.value;
    if (message?.trim()) {
      ctrl?.setValue('');
      this.chatService.sendMessage(message);
    }
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
