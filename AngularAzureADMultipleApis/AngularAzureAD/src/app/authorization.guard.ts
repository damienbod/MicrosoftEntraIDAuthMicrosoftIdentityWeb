import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthorizationGuard implements CanActivate {
    constructor(private oidcSecurityService: OidcSecurityService, private router: Router) {}

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> {
        return this.oidcSecurityService.isAuthenticated$.pipe(
            map(({ isAuthenticated }) => {
                console.log('AuthorizationGuard, canActivate isAuthorized: ' + isAuthenticated);

                if (!isAuthenticated) {
                    this.router.navigate(['/unauthorized']);
                    return false;
                }

                return true;
            })
        );
    }
}
