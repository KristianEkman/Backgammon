import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent implements OnInit {
  @Input() default = false;
  @Input() type = 'button';
  constructor(private cd: ChangeDetectorRef) {
    this.theme = AppState.Singleton.theme.observe();
  }

  ngOnInit(): void {
    setTimeout(() => {
      AppState.Singleton.theme.next();
    }, 1);
  }

  theme: Observable<string>;
}
