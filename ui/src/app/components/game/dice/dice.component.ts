import { NgStyle } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-dice',
  templateUrl: './dice.component.html',
  styleUrls: ['./dice.component.scss']
})
export class DiceComponent {
  @Input() color: 'black' | 'white' | undefined;

  constructor() {
    setInterval(() => {
      this.sideNo++;
      if (this.sideNo > 6) this.sideNo = 0;
      this.side = this.sides[this.sideNo];
    }, 2000);
  }

  sideNo = 0;

  sides = [
    'show-front',
    'show-right',
    'show-top',
    'show-left',
    'show-back',
    'show-bottom'
  ];

  side = '';

  getColorStyle(): any {
    if (this.color == 'black') {
      return { backgroundColor: '#111', color: '#ccc' };
    }
    return { backgroundColor: '#ccc', color: '#111' };
  }
}
