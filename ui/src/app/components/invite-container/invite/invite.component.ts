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
import { InputCopyComponent } from '../../shared/input-copy/input-copy.component';
import { ButtonComponent } from '../../shared/button/button.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-invite',
  standalone: true,
  templateUrl: './invite.component.html',
  styleUrls: ['./invite.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [InputCopyComponent, ButtonComponent, TranslateModule]
})
export class InviteComponent implements OnChanges {
  @Input() gameId: string | undefined = '';
  @Output() cancel = new EventEmitter<string>();
  @Output() started = new EventEmitter<string>();
  @ViewChild('linkText', { static: false }) linkText: ElementRef | undefined;

  link = '';

  ngOnChanges(): void {
    this.link = `${window.location.href}?${Keys.inviteId}=${this.gameId}`;
  }

  startClick(): void {
    this.started.emit(this.gameId);
  }

  cancelClick(): void {
    this.cancel.emit(this.gameId);
  }
}
