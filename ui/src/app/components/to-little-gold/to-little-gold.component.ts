import { ChangeDetectionStrategy, Component } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-to-little-gold',
  standalone: true,
  templateUrl: './to-little-gold.component.html',
  styleUrls: ['./to-little-gold.component.scss'],
  imports: [TranslateModule],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToLittleGoldComponent {}
