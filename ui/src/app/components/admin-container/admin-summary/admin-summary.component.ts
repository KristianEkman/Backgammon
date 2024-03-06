import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { SummaryDto } from 'src/app/dto';

@Component({
  selector: 'app-admin-summary',
  standalone: true,
  templateUrl: './admin-summary.component.html',
  styleUrls: ['./admin-summary.component.scss'],
  imports: [CommonModule]
})
export class AdminSummaryComponent {
  @Input() summary: SummaryDto | null;
  constructor() {
    this.summary = null;
  }
}
