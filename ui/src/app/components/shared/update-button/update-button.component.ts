import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-update-button',
  templateUrl: './update-button.component.html',
  styleUrls: ['./update-button.component.scss']
})
export class UpdateButtonComponent implements OnInit {
  @Input() updateAvailable: boolean | null = false;
  @Output() update = new EventEmitter<void>();

  ngOnInit(): void {}

  updateClick() {
    this.update.emit();
  }
}
