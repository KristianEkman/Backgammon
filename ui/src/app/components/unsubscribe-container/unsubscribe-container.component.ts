import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { take } from 'rxjs/operators';
import { MessageService } from 'src/app/services';
import { Busy } from 'src/app/state/busy';

@Component({
  selector: 'app-unsubscribe-container',
  templateUrl: './unsubscribe-container.component.html',
  styleUrls: ['./unsubscribe-container.component.scss']
})
export class UnsubscribeContainerComponent implements OnInit {
  constructor(messageService: MessageService, route: ActivatedRoute) {
    Busy.show();
    route.queryParams.subscribe((params) => {
      const unsubid = params.id;
      messageService
        .emailUnsubscribe(unsubid)
        .pipe(take(1))
        .subscribe(
          () => {
            this.show = true;
            Busy.hide();
          },
          (err) => {
            this.hasError = true;
            Busy.hide();
            throw err;
          }
        );
    });
  }

  show = false;
  hasError = false;

  ngOnInit(): void {}
}
