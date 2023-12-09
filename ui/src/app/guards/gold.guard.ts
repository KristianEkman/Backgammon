import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class GoldGuard  {
  constructor(private router: Router, private appState: AppStateService) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    routerState: RouterStateSnapshot
  ):
    | Observable<boolean | UrlTree>
    | Promise<boolean | UrlTree>
    | boolean
    | UrlTree {
    const user = this.appState.user.getValue();
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
