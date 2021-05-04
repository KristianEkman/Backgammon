import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-about-container',
  templateUrl: './about-container.component.html',
  styleUrls: ['./about-container.component.scss']
})
export class AboutContainerComponent implements OnInit {
  constructor() {}

  ngOnInit(): void {}

  diceValue = 1;
  diceColor: 'black' | 'white' = 'black';

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
