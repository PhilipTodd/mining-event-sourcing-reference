import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

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
  private readonly baseUrl = 'https://localhost:7221';

  constructor(private readonly http: HttpClient) { }

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
}
