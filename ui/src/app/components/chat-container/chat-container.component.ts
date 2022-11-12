import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-chat-container',
  templateUrl: './chat-container.component.html',
  styleUrls: ['./chat-container.component.scss']
})
export class ChatContainerComponent implements OnInit {
  open = false;

  ngOnInit(): void {}

  @ViewChild('msginput') input!: ElementRef;

  toggle() {
    this.open = !this.open;
    if (this.open) {
      setTimeout(() => {
        this.input?.nativeElement?.focus();
      }, 1);

      console.log(this.input);
    }
  }
}
