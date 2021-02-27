import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-board-buttons',
  templateUrl: './board-buttons.component.html',
  styleUrls: ['./board-buttons.component.scss']
})
export class BoardButtonsComponent {
  @Input() undoVisible = false;
  @Input() sendVisible = false;
  @Input() rollButtonVisible = false;
  @Input() newVisible = false;

  @Output() onUndoMove = new EventEmitter<void>();
  @Output() onSendMoves = new EventEmitter<void>();
  @Output() onRoll = new EventEmitter<void>();
  @Output() onNew = new EventEmitter<void>();

  undoMove(): void {
    this.onUndoMove.emit();
  }

  sendMoves(): void {
    this.onSendMoves.emit();
  }

  rollButtonClick(): void {
    this.onRoll.emit();
  }

  newGame(): void {
    this.onNew.emit();
  }
}
