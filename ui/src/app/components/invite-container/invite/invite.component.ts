import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  ViewChild
} from '@angular/core';
import { Keys } from 'src/app/utils';

@Component({
  selector: 'app-invite',
  templateUrl: './invite.component.html',
  styleUrls: ['./invite.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class InviteComponent implements OnChanges {
  @Input() gameId: string | undefined = '';
  @Output() cancel = new EventEmitter<string>();
  @Output() start = new EventEmitter<string>();
  @ViewChild('linkText', { static: false }) linkText: ElementRef | undefined;

  link = '';

  ngOnChanges(): void {
    this.link = `${window.location.href}?${Keys.inviteId}=${this.gameId}`;
  }

  startClick(): void {
    this.start.emit(this.gameId);
  }

  cancelClick(): void {
    this.cancel.emit(this.gameId);
  }
}
