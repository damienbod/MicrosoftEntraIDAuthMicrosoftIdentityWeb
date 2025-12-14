import { AuthenticatedResult } from 'angular-auth-oidc-client';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from '../auth.service';

@Component({
    selector: 'app-nav-menu',
    templateUrl: './nav-menu.component.html',
    styleUrls: ['./nav-menu.component.css'],
    standalone: false
})
export class NavMenuComponent implements OnInit {
  userData$: Observable<any>;
  isAuthenticated$: Observable<AuthenticatedResult>;
  
  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.userData$ = this.authService.userData$;
    this.isAuthenticated$ = this.authService.signedIn$;
  }

  login() {
    this.authService.signIn();
  }

  forceRefreshSession() {
    this.authService.forceRefreshSession().subscribe((data) => {
      console.log('Refresh completed');
    });
  }

  logout() {
    this.authService.signOut().subscribe((result) => {
      console.log('Logout completed');
    });
  }
}
