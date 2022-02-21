import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AuthenticatedResult } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-delegated-api-call',
  templateUrl: 'delegatedApiCall.component.html',
})
export class DelegatedApiCallComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated$: Observable<AuthenticatedResult>;
  httpRequestRunning = false;
  constructor(
    private authService: AuthService,
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
    this.userData$ = this.authService.userData$;
    this.isAuthenticated$ = this.authService.signedIn$;
  }

  callApi() {
    this.httpRequestRunning = true;
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44390/DelegatedUserApiCalls')
      .pipe(finalize(() => (this.httpRequestRunning = false)));
  }
}
