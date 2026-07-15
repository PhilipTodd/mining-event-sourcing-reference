import {
  ChangeDetectionStrategy,
  Component,
  DestroyRef,
  OnInit,
  inject,
  signal
} from '@angular/core';
import { DatePipe } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs';

import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';


import { BlastPlanSummary } from '../../core/models/blast-plan.models';
import { BlastPlanApiService } from '../../core/services/blast-plan-api.service';

import {BlastPlanListRefreshService} from '../../core/services/blast-plan-list-refresh.service';

@Component({
  selector: 'app-blast-plan-list',
  standalone: true,
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTableModule
  ],
  templateUrl: './blast-plan-list.html',
  styleUrl: './blast-plan-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BlastPlanListComponent implements OnInit {
  private readonly blastPlanService = inject(BlastPlanApiService);
  private readonly refreshService = inject(BlastPlanListRefreshService);
  private readonly destroyRef = inject(DestroyRef);

  readonly blastPlans = signal<BlastPlanSummary[]>([]);
  readonly isLoading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly displayedColumns = [
    'name',
    'siteId',
    'status',
    'createdUtc',
    'approvedUtc'
  ];

  ngOnInit(): void {
    this.refreshService.refreshRequested$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.loadBlastPlans());

    this.loadBlastPlans();
  }

  loadBlastPlans(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.blastPlanService
      .getRecentBlastPlans()
      .pipe(
        finalize(() => {
          this.isLoading.set(false);
        })
      )
      .subscribe({
        next: blastPlans => {
          this.blastPlans.set(blastPlans);
        },
        error: error => {
          console.error('Failed to load blast plans.', error);

          this.blastPlans.set([]);
          this.errorMessage.set(
            'Blast plans could not be loaded. Please try again.'
          );
        }
      });
  }

  trackByBlastPlanId(
    index: number,
    blastPlan: BlastPlanSummary
  ): string {
    return blastPlan.blastPlanId;
  }
}