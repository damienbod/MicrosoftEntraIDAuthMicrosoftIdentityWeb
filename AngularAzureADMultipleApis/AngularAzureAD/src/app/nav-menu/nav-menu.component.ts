import { Component, OnInit } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
})
export class NavMenuComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated$: Observable<boolean>;
  constructor(
    private authservice: AuthService,
  ) {}

  ngOnInit() {
    this.userData$ = this.authservice.userData;
    this.isAuthenticated$ = this.authservice.signedIn;
  }

  login() {
    this.authservice.signIn();
  }

  forceRefreshSession() {
    this.authservice.forceRefreshSession().subscribe((data) => {
      console.log('Refresh completed');
    });
  }

  logout() {
    this.authservice.signOut();
  }
}
