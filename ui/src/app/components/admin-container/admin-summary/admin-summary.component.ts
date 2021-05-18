import { Component, Input, OnInit } from '@angular/core';
import { SummaryDto } from 'src/app/dto';

@Component({
  selector: 'app-admin-summary',
  templateUrl: './admin-summary.component.html',
  styleUrls: ['./admin-summary.component.scss']
})
export class AdminSummaryComponent implements OnInit {
  @Input() summary: SummaryDto | null;
  constructor() {
    this.summary = null;
  }

  ngOnInit(): void {}
}
