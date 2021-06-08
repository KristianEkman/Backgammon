import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree
} from '@angular/router';
import { Observable } from 'rxjs';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class GoldGuard implements CanActivate {
  constructor(private router: Router) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    routerState: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const user = AppState.Singleton.user.getValue();
    const urlTree = this.router.parseUrl(routerState.url);
    const forGold = urlTree.queryParams['forGold'] === 'true';

    if (!user && forGold) {
      this.router.navigateByUrl('/lobby');
      return false;
    }

    if (user?.gold < 50 && forGold) {
      this.router.navigateByUrl('/tolittlegold');
    }

    return true;
  }
}
