import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { switchMap, take } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private secureRoutes = ['https://localhost:44390'];

  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (!this.secureRoutes.find((x) => request.url.startsWith(x))) {
      return next.handle(request);
    }

    return this.authService.token$.pipe(
      take(1),
      switchMap((token) => {
        if (!token) {
          return next.handle(request);
        }

        const clonedRequest = request.clone({
          headers: request.headers.set('Authorization', 'Bearer ' + token),
        });

        return next.handle(clonedRequest);
      })
    );
  }
}
