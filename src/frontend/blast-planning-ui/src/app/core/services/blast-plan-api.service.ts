import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';

import {
  ApproveBlastPlanRequest,
  BlastPlanSummary,
  CreateBlastPlanRequest,
  CreateBlastPlanResult
} from '../models/blast-plan.models';

@Injectable({
  providedIn: 'root'
})
export class BlastPlanApiService {
  private readonly http = inject(HttpClient);

  private readonly baseUrl = environment.apiBaseUrl;

  createBlastPlan(request: CreateBlastPlanRequest) {
    return this.http.post<CreateBlastPlanResult>(
      `${this.baseUrl}/blast-plans`,
      request
    );
  }

  approveBlastPlan(blastPlanId: string, request: ApproveBlastPlanRequest) {
    return this.http.post<void>(
      `${this.baseUrl}/blast-plans/${blastPlanId}/approve`,
      request
    );
  }

  getBlastPlan(blastPlanId: string) {
    return this.http.get<BlastPlanSummary>(
      `${this.baseUrl}/blast-plans/${blastPlanId}`
    );
  }

  getRecentBlastPlans(): Observable<BlastPlanSummary[]> {
    return this.http.get<BlastPlanSummary[]>(
      `${environment.apiBaseUrl}/blast-plans/recent`
    );
  }  
}
