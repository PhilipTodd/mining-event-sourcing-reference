import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MsalService } from '@azure/msal-angular';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-app-shell',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    MatSidenavModule,
    MatToolbarModule
  ],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss'
})
export class AppShell {
  private readonly msal = inject(MsalService);

  constructor(private readonly cdr: ChangeDetectorRef) {
    this.msal.handleRedirectObservable().subscribe({
      next: result => {
        if (result?.account) {
          this.msal.instance.setActiveAccount(result.account);
        } else {
          const account = this.msal.instance.getAllAccounts()[0];
          if (account) {
            this.msal.instance.setActiveAccount(account);
          }
        }

        this.cdr.detectChanges();
      },
      error: error => {
        console.error('MSAL redirect handling failed', error);
        this.cdr.detectChanges();
      }
    });
  }

  isAuthenticated(): boolean {
    return this.msal.instance.getAllAccounts().length > 0;
  }

  login(): void {
    this.msal.loginRedirect({
      scopes: ['api://9b84c3bc-479f-4f57-b5eb-8efef1f6e062/blastplans.write']
    });
  }

  logout(): void {
    this.msal.instance.setActiveAccount(null);
    this.cdr.detectChanges();

    this.msal.logoutRedirect({
      postLogoutRedirectUri: window.location.origin
    });
  }

  get account() {
    return this.msal.instance.getActiveAccount()
      ?? this.msal.instance.getAllAccounts()[0]
      ?? null;
  }

  get userName(): string {
    return this.account?.name
      ?? this.account?.idTokenClaims?.['name'] as string
      ?? 'Signed in';
  }

  get userEmail(): string {
    return this.account?.username
      ?? this.account?.idTokenClaims?.['preferred_username'] as string
      ?? '';
  }

}
