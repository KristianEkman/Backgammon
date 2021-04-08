import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home-button',
  templateUrl: './home-button.component.html',
  styleUrls: ['./home-button.component.scss']
})
export class HomeButtonComponent implements OnInit {
  constructor(public router: Router) {}

  ngOnInit(): void {}
}
