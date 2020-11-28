import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-graph-api-call',
  templateUrl: 'graphApiCall.component.html',
})
export class GraphApiCallComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated$: Observable<boolean>;
  httpRequestRunning = false;
  constructor(
    private authservice: AuthService,
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
    this.userData$ = this.authservice.userData;
    this.isAuthenticated$ = this.authservice.signedIn;
  }

  callApi() {
    this.httpRequestRunning = true;
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44390/GraphApiCalls')
      .pipe(
        catchError((error) => {
          this.httpRequestRunning = false;
          return of(error);
        }),
        switchMap((response: any) => {
          this.httpRequestRunning = false;
          return of(response);
        })
      );
  }

}