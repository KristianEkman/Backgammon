import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-dice',
  templateUrl: './dice.component.html',
  styleUrls: ['./dice.component.scss']
})
export class DiceComponent implements OnChanges {
  @Input() color: 'black' | 'white' | undefined;
  @Input() value = 1;
  @Input() disabled = false;

  constructor() {}

  sideNo = 0;

  sides = [
    'show-front',
    'show-right',
    'show-top',
    'show-bottom',
    'show-left',
    'show-back'
  ];

  side = '';

  randomTransform = {};

  getColorStyle(v: number): any {
    let c = 'w';
    let color = 'white';
    if (this.color == 'black') {
      c = 'b';
      color = 'black';
    }

    return {
      backgroundColor: color,
      backgroundImage: `url(/assets/images/dice/${c}${v}.png)`,
      backgroundSize: 'calc(100% - 12px)',
      backgroundPosition: 'center',
      backgroundRepeat: 'no-repeat',
      border: '1px solid #777'
    };
  }

  getDisabledStyle(): any {
    if (this.disabled) {
      return {
        opacity: '0.2'
      };
    }
    return {
      opacity: '0.8'
    };
  }

  ngOnChanges(change: SimpleChanges): void {
    if (change['value']) {
      const x = this.randomIntFromInterval(200, 400);
      const y = this.randomIntFromInterval(200, 400);
      const z = this.randomIntFromInterval(200, 400);
      this.randomTransform = {
        transform: `translateZ(100px) rotateX(${x}deg) rotateY(${y}deg) rotateZ(${z}deg)`
      };

      setTimeout(() => {
        this.randomTransform = {};
        this.side = this.sides[this.value - 1];
      }, 3000);
    }
  }

  // min and max included
  randomIntFromInterval(min: number, max: number): number {
    return Math.floor(Math.random() * (max - min + 1) + min);
  }
}
