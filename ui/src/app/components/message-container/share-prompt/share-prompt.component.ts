import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ShareButtonsComponent } from '../../shared/share-buttons/share-buttons.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-share-prompt',
  standalone: true,
  templateUrl: './share-prompt.component.html',
  styleUrls: ['./share-prompt.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ShareButtonsComponent, TranslateModule]
})
export class SharePromptComponent {}
