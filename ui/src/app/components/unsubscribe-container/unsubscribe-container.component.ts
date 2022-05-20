import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { MessageService } from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-unsubscribe-container',
  templateUrl: './unsubscribe-container.component.html',
  styleUrls: ['./unsubscribe-container.component.scss']
})
export class UnsubscribeContainerComponent implements OnInit {
  constructor(
    messageService: MessageService,
    route: ActivatedRoute,
    private appState: AppStateService
  ) {
    this.appState.showBusy();
    route.queryParams.subscribe((params) => {
      const unsubid = params.id;
      messageService
        .emailUnsubscribe(unsubid)
        .pipe(take(1))
        .subscribe(
          () => {
            this.show = true;
            this.appState.hideBusy();
          },
          (err) => {
            this.hasError = true;
            this.appState.hideBusy();

            throw err;
          }
        );
    });
  }

  show = false;
  hasError = false;

  ngOnInit(): void {}
}
