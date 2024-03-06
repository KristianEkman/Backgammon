import { Component } from '@angular/core';
import { default as packageJson } from '../../../../package.json';
import { CommonModule } from '@angular/common';
import { ShareButtonsComponent } from '../shared/share-buttons/share-buttons.component';
import { TranslateModule } from '@ngx-translate/core';
import { DiceComponent } from '../game/dice/dice.component';

@Component({
  selector: 'app-about-container',
  standalone: true,
  templateUrl: './about-container.component.html',
  styleUrls: ['./about-container.component.scss'],
  imports: [CommonModule, ShareButtonsComponent, TranslateModule, DiceComponent]
})
export class AboutContainerComponent {
  diceValue = 1;
  diceColor: 'black' | 'white' = 'black';

  version = packageJson.version;
  showInfo = false;

  onClick(): void {
    let d = this.diceValue;
    d++;
    if (d > 6) d = 1;
    this.diceValue = d;
  }

  onDoubleClick(): void {
    if (this.diceColor === 'black') {
      this.diceColor = 'white';
    } else {
      this.diceColor = 'black';
    }
  }
}
