import { Component, Input } from '@angular/core';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-theme',
  templateUrl: './theme.component.html',
  styleUrls: ['./theme.component.scss']
})
export class ThemeComponent {
  ngOnInit(): void {}

  @Input() currentTheme: string | undefined = 'dark';

  themes = AppStateService.Themes;
}
