import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class GoldGuard implements CanActivate {
  constructor(private router: Router) {}
  canActivate():
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const user = AppState.Singleton.user.getValue();

    if (!user) {
      this.router.navigateByUrl('/lobby');
      return false;
    }

    if (user.gold < 50) {
      this.router.navigateByUrl('/tolittlegold');
    }

    return true;
  }
}
