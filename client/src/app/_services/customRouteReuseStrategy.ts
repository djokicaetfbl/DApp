import {
  ActivatedRouteSnapshot,
  DetachedRouteHandle,
  RouteReuseStrategy,
} from '@angular/router';

export class CustomRouteReuseStrategy implements RouteReuseStrategy {
  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }
  store(
    route: ActivatedRouteSnapshot,
    handle: DetachedRouteHandle | null
  ): void {}
  shouldAttach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
    return null;
  }
  shouldReuseRoute(
    future: ActivatedRouteSnapshot,
    curr: ActivatedRouteSnapshot
  ): boolean {
    return false;
  } /* bio je problem ako sam na tabu nekog korisnika A, a dobio poruku od korisnika B, kad kliknem da vidim poruku, ono me prebaci na message tab od korisnika A umjesto B, sa ovim false, spijecen je reuse rute */
}
