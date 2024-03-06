import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Output
} from '@angular/core';
import { ButtonComponent } from '../../shared/button/button.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-play-ai-question',
  standalone: true,
  templateUrl: './play-ai-question.component.html',
  styleUrls: ['./play-ai-question.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ButtonComponent, TranslateModule]
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
