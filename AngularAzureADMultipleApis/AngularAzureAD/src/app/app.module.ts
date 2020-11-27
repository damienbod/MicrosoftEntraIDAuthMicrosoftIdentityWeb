import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { AuthModule, OidcConfigService, LogLevel } from 'angular-auth-oidc-client';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';

export function configureAuth(oidcConfigService: OidcConfigService) {
  return () =>
    oidcConfigService.withConfig({
            stsServer: 'https://login.microsoftonline.com/7ff95b15-dc21-4ba6-bc92-824856578fc1/v2.0',
            authWellknownEndpoint: 'https://login.microsoftonline.com/7ff95b15-dc21-4ba6-bc92-824856578fc1/v2.0',
            redirectUrl: window.location.origin,
            clientId: 'ad6b0351-92b4-4ee9-ac8d-3e76e5fd1c67',
            scope: 'openid profile email api://98328d53-55ec-4f14-8407-0ca5ff2f2d20/access_as_user',
            responseType: 'code',
            silentRenew: true,
            maxIdTokenIatOffsetAllowedInSeconds: 600,
            issValidationOff: false, // this needs to be true if using a common endpoint in Azure
            autoUserinfo: false,
            silentRenewUrl: window.location.origin + '/silent-renew.html',
            logLevel: LogLevel.Debug
    });
}

@NgModule({
  declarations: [AppComponent, HomeComponent, UnauthorizedComponent],
  imports: [
    BrowserModule,
    RouterModule.forRoot([
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'unauthorized', component: UnauthorizedComponent },
], { relativeLinkResolution: 'legacy' }),
    AuthModule.forRoot(),
    HttpClientModule,
  ],
  providers: [
    OidcConfigService,
    {
      provide: APP_INITIALIZER,
      useFactory: configureAuth,
      deps: [OidcConfigService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
