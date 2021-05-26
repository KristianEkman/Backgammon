import { Component, Input, OnInit } from '@angular/core';
import { PlayerDto } from 'src/app/dto';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit {
  constructor() {}

  @Input() playerDto?: PlayerDto;
  @Input() doubling: number | null = null;

  ngOnInit(): void {}
}
