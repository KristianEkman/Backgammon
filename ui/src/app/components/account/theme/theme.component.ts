import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-theme',
  standalone: true,
  templateUrl: './theme.component.html',
  styleUrls: ['./theme.component.scss'],
  imports: [CommonModule, TranslateModule]
})
export class ThemeComponent {
  @Input() currentTheme: string | undefined = 'dark';

  themes = AppStateService.Themes;
}
