import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private oidcSecurityService: OidcSecurityService) {}

  get signedIn$() {
    return this.oidcSecurityService.isAuthenticated$;
  }

  get token$(): Observable<string> {
    return this.oidcSecurityService.getAccessToken();
  }

  get userData$() {
    return this.oidcSecurityService.userData$;
  }

  checkAuth() {
    return this.oidcSecurityService.checkAuth();
  }

  signIn() {
    this.oidcSecurityService.authorize();
  }

  signOut() {
    return this.oidcSecurityService.logoff();
  }

  forceRefreshSession() {
    return this.oidcSecurityService.forceRefreshSession();
  }
}
