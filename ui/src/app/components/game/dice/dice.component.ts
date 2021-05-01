import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dice',
  templateUrl: './dice.component.html',
  styleUrls: ['./dice.component.scss']
})
export class DiceComponent implements OnInit {
  constructor() {
    setInterval(() => {
      // const i = this.getRandomInt(0, 6);
      // this.side = this.sides[i];

      this.sideNo++;
      if (this.sideNo > 6) this.sideNo = 0;
      this.side = this.sides[this.sideNo];
    }, 2000);
  }

  ngOnInit(): void {}
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

  getRandomInt(min: number, max: number): number {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min) + min); //The maximum is exclusive and the minimum is inclusive
  }
}
