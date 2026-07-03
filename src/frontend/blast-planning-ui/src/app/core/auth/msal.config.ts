import {
  IPublicClientApplication,
  PublicClientApplication,
  InteractionType
} from '@azure/msal-browser';

import {
  MsalGuardConfiguration,
  MsalInterceptorConfiguration
} from '@azure/msal-angular';

import { environment } from '../../../environments/environment.development';

export function msalInstanceFactory(): IPublicClientApplication {
  return new PublicClientApplication({
    auth: {
      clientId: environment.entra.clientId,
      authority: `https://login.microsoftonline.com/${environment.entra.tenantId}`,
      redirectUri: '/'
    }
  });
}

export function msalInterceptorConfigFactory(): MsalInterceptorConfiguration {

  const protectedResourceMap = new Map<string, Array<string>>();

  protectedResourceMap.set(
    environment.apiBaseUrl,
    [environment.entra.apiScope]
  );

  return {
    interactionType: InteractionType.Popup,
    protectedResourceMap
  };
}

export function msalGuardConfigFactory(): MsalGuardConfiguration {

  return {
    interactionType: InteractionType.Popup,
    authRequest: {
      scopes: [environment.entra.apiScope]
    }
  };
}
