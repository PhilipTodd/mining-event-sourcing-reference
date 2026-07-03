import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MsalService } from '@azure/msal-angular';

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

  isAuthenticated(): boolean {
    return this.msal.instance.getAllAccounts().length > 0;
  }

  login(): void {
    this.msal.instance.initialize().then(() => {
      this.msal.loginRedirect({
        scopes: ['api://9b84c3bc-479f-4f57-b5eb-8efef1f6e062/blastplans.write']
      });
    });
  }

  logout(): void {
    this.msal.logoutRedirect();
  }
}
