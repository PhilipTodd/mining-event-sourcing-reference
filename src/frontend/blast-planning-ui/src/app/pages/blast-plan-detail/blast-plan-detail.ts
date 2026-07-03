import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MsalService } from '@azure/msal-angular';
import { ChangeDetectorRef } from '@angular/core';

import { BlastPlanSummary } from '../../core/models/blast-plan.models';
import { BlastPlanApiService } from '../../core/services/blast-plan-api.service';

@Component({
  selector: 'app-blast-plan-detail',
  imports: [
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatSnackBarModule
  ],
  templateUrl: './blast-plan-detail.html',
  styleUrl: './blast-plan-detail.scss'
})
export class BlastPlanDetail implements OnInit {
  blastPlanId = '';
  summary: BlastPlanSummary | null = null;
  isLoading = false;
  isApproving = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly api: BlastPlanApiService,
    private readonly snackBar: MatSnackBar,
    private readonly msal: MsalService,
    private readonly cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.blastPlanId = this.route.snapshot.paramMap.get('id') ?? '';
    this.load();
  }

  load(): void {
    this.isLoading = true;
    this.summary = null;

    this.api.getBlastPlan(this.blastPlanId).subscribe({
      next: result => {
        this.summary = result;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        setTimeout(() => this.loadOnceMore(), 1000);
      }
    });
  }

  private loadOnceMore(): void {
    this.api.getBlastPlan(this.blastPlanId).subscribe({
      next: result => {
        this.summary = result;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.summary = null;
        this.snackBar.open('Projection is not available yet', 'Dismiss', { duration: 4000 });
        this.cdr.detectChanges();
      }
    });
  }

  get loggedIn(): boolean {
    return this.msal.instance.getAllAccounts().length > 0;
  }

  approve(): void {
    this.isApproving = true;

    this.api.approveBlastPlan(this.blastPlanId, {
      approvedBy: 'demo-user'
    }).subscribe({
      next: () => {
        this.snackBar.open('Blast plan approval submitted', 'Dismiss', { duration: 3000 });
        this.isApproving = false;
        this.cdr.detectChanges();
        setTimeout(() => this.load(), 1500);
      },
      error: () => {
        this.isApproving = false;
        this.snackBar.open('Failed to approve blast plan', 'Dismiss', { duration: 5000 });
        this.cdr.detectChanges();
      }
    });
  }
}
