import { Injectable } from '@angular/core';
import { Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class LoginGuard  {
  constructor(private router: Router, private appState: AppStateService) {}
  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const ok = !!this.appState.user.getValue();

    if (!ok) {
      this.router.navigateByUrl('/lobby');
    }
    return ok;
  }
}
