import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthenticatedResult } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
})
export class HomeComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated = false;
  constructor(
    private authService: AuthService,
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
    this.userData$ = this.authService.userData$;
    this.authService.signedIn$.subscribe(({ isAuthenticated }) => {
      this.isAuthenticated = isAuthenticated;

      console.warn('authenticated: ', isAuthenticated);
    });
  }

  callApi() {
    this.dataFromAzureProtectedApi$ = this.httpClient.get(
      'https://localhost:44390/DirectApi'
    );
  }
}
