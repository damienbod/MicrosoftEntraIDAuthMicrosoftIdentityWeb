import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../auth.service';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-direct-api-call',
  templateUrl: 'directApiCall.component.html',
})
export class DirectApiCallComponent implements OnInit {
  userData$: Observable<any>;
  dataFromAzureProtectedApi$: Observable<any>;
  isAuthenticated$: Observable<boolean>;
  constructor(
    private authservice: AuthService,
    private httpClient: HttpClient
  ) {}

  ngOnInit() {
    this.userData$ = this.authservice.userData;
    this.isAuthenticated$ = this.authservice.signedIn;
  }

  callApi() {
    this.dataFromAzureProtectedApi$ = this.httpClient
      .get('https://localhost:44390/DirectApi')
      .pipe(catchError((error) => of(error)));
  }

}