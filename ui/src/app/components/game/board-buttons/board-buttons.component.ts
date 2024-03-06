import { CommonModule } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-board-buttons',
  standalone: true,
  templateUrl: './board-buttons.component.html',
  styleUrls: ['./board-buttons.component.scss'],
  imports: [CommonModule, TranslateModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BoardButtonsComponent {
  @Input() undoVisible = false;
  @Input() sendVisible = false;
  @Input() rollButtonVisible = false;
  @Input() newVisible = false;
  @Input() exitVisible = true;
  @Input() acceptDoublingVisible = false;
  @Input() requestDoublingVisible = false;
  @Input() doublingFactor: number | null = 1;
  @Input() requestHintVisible = false;

  @Output() onUndoMove = new EventEmitter<void>();
  @Output() onSendMoves = new EventEmitter<void>();
  @Output() onRoll = new EventEmitter<void>();
  @Output() onNew = new EventEmitter<void>();
  @Output() onExit = new EventEmitter<void>();
  @Output() onAcceptDoubling = new EventEmitter<void>();
  @Output() onRequestDoubling = new EventEmitter<void>();
  @Output() onResign = new EventEmitter<void>();
  @Output() onRequestHint = new EventEmitter<void>();

  undoMove(): void {
    this.onUndoMove.emit();
  }

  sendMoves(): void {
    this.sendVisible = false;
    this.onSendMoves.emit();
  }

  rollButtonClick(): void {
    this.onRoll.emit();
  }

  newGame(): void {
    this.onNew.emit();
  }

  exitGame(): void {
    this.onExit.emit();
  }

  acceptDoubling(): void {
    this.onAcceptDoubling.emit();
  }

  requestDoubling(): void {
    this.onRequestDoubling.emit();
  }

  resign(): void {
    this.onResign.emit();
  }

  requestHint(): void {
    this.onRequestHint.emit();
  }
}
