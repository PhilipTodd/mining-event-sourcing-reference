import { APP_INITIALIZER } from '@angular/core';
import { MsalService } from '@azure/msal-angular';
import { ApplicationConfig, importProvidersFrom } from '@angular/core';

import {
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection
} from '@angular/core';

import { provideRouter } from '@angular/router';

import {
  provideHttpClient,
  withInterceptorsFromDi,
  HTTP_INTERCEPTORS
} from '@angular/common/http';

import {
  MsalGuard,
  MsalInterceptor,
  MsalModule
} from '@azure/msal-angular';

import {
  BrowserCacheLocation,
  InteractionType,
  PublicClientApplication
} from '@azure/msal-browser';

import { routes } from './app.routes';
import { environment } from '../environments/environment';
export function initializeMsal(msalService: MsalService): () => Promise<void> {
  return () => msalService.instance.initialize();
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),

    importProvidersFrom(
      MsalModule.forRoot(
        new PublicClientApplication({
          auth: {
            clientId: environment.entra.clientId,
            authority: `https://login.microsoftonline.com/${environment.entra.tenantId}`,
            redirectUri: environment.entra.redirectUri
          },
          cache: {
            cacheLocation: BrowserCacheLocation.LocalStorage
          }
        }),
        {
          interactionType: InteractionType.Redirect,
          authRequest: {
            scopes: [environment.entra.apiScope]
          }
        },
        {
          interactionType: InteractionType.Redirect,

          protectedResourceMap: new Map([
            [
              `${environment.apiBaseUrl}/blast-plans`,
              [environment.entra.apiScope]
            ],
            [
              `${environment.apiBaseUrl}/blast-plans/`,
              [environment.entra.apiScope]
            ],
            [
              `${environment.apiBaseUrl}/blast-plans/*/approve`,
              [environment.entra.apiScope]
            ]
          ])

          // protectedResourceMap: new Map([
          //   [
          //     'https://localhost:7221/blast-plans',
          //     ['api://9b84c3bc-479f-4f57-b5eb-8efef1f6e062/blastplans.write']
          //   ],
          //   [
          //     'https://localhost:7221/blast-plans/',
          //     ['api://9b84c3bc-479f-4f57-b5eb-8efef1f6e062/blastplans.write']
          //   ],
          //   [
          //     'https://localhost:7221/blast-plans/*/approve',
          //     ['api://9b84c3bc-479f-4f57-b5eb-8efef1f6e062/blastplans.write']
          //   ]
          // ])
        }
      )
    ),

    {
      provide: APP_INITIALIZER,
      useFactory: initializeMsal,
      deps: [MsalService],
      multi: true
    },

    {
      provide: HTTP_INTERCEPTORS,
      useClass: MsalInterceptor,
      multi: true
    },

    MsalGuard
  ]
};
