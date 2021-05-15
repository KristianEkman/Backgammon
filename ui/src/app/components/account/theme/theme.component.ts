import { Component, Input } from '@angular/core';
import { Theme } from './theme';

@Component({
  selector: 'app-theme',
  templateUrl: './theme.component.html',
  styleUrls: ['./theme.component.scss']
})
export class ThemeComponent {
  ngOnInit(): void {}

  @Input() currentTheme: string | undefined = 'dark';

  themes = Theme.Themes;
}
