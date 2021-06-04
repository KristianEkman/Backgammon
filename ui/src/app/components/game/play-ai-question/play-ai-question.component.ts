import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Output
} from '@angular/core';

@Component({
  selector: 'app-play-ai-question',
  templateUrl: './play-ai-question.component.html',
  styleUrls: ['./play-ai-question.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PlayAiQuestionComponent {
  @Output() onPlayAi = new EventEmitter<void>();
  @Output() onKeepWaiting = new EventEmitter<void>();

  continueWait(): void {
    this.onKeepWaiting.emit();
  }

  playAi(): void {
    this.onPlayAi.emit();
  }
}
