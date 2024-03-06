import {
  Component,
  ElementRef,
  Input,
  OnChanges,
  ViewChild
} from '@angular/core';

@Component({
  selector: 'app-input-copy',
  standalone: true,
  templateUrl: './input-copy.component.html',
  styleUrls: ['./input-copy.component.scss']
})
export class InputCopyComponent implements OnChanges {
  constructor() {}

  @ViewChild('linkText', { static: false }) linkText: ElementRef | undefined;
  @Input() text: string | null = '';

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
