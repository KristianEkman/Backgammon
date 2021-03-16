import { Component, Input, OnInit } from '@angular/core';
import { Busy } from 'src/app/state/busy';

@Component({
  selector: 'app-busy',
  templateUrl: './busy.component.html',
  styleUrls: ['./busy.component.scss']
})
export class BusyComponent implements OnInit {
  constructor() {}

  @Input() busy: Busy | null = null;
  @Input() text = 'Please wait.';
  @Input() overlay = true;

  ngOnInit(): void {}

  ngOnChanges(): void {
    console.log(this.busy);
  }
}
