import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SummaryDto } from 'src/app/dto';

@Component({
  selector: 'app-admin-summary',
  templateUrl: './admin-summary.component.html',
  styleUrls: ['./admin-summary.component.scss']
})
export class AdminSummaryComponent implements OnInit {
  @Input() summary: SummaryDto | null;
  @Output() onShowGame = new EventEmitter<void>();
  constructor() {
    this.summary = null;
  }

  ngOnInit(): void {}

  showAllGames(): void {
    this.onShowGame.emit();
  }
}
