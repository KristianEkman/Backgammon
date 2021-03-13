import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Toplist } from 'src/app/dto';

@Component({
  selector: 'app-toplist',
  templateUrl: './toplist.component.html',
  styleUrls: ['./toplist.component.scss']
})
export class ToplistComponent implements OnInit {
  @Input() toplist: Toplist | null = null;
  @Output() closed = new EventEmitter<void>();
  constructor() {}

  ngOnInit(): void {}

  closeClick(): void {
    this.closed.emit();
  }
}
