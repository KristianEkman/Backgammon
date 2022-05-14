import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';

@Component({
  selector: 'app-input-copy',
  templateUrl: './input-copy.component.html',
  styleUrls: ['./input-copy.component.scss']
})
export class InputCopyComponent implements OnInit {
  constructor() {}

  @ViewChild('linkText', { static: false }) linkText: ElementRef | undefined;
  @Input() text: string | null = '';

  ngOnInit(): void {}

  ngOnChanges() {
    if (this.text) {
      setTimeout(() => {
        this.selectAndCopy();
      }, 1);
    }
  }

  selectAndCopy(): void {
    setTimeout(() => {
      const input = this.linkText?.nativeElement as HTMLInputElement;
      input.focus();
      input.select();
      input.setSelectionRange(0, 99999); /* For mobile devices */
      navigator.clipboard.writeText(input.value);
      // document.execCommand('copy');
    }, 1);
  }
}
