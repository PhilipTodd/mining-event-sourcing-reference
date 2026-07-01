export interface CreateBlastPlanRequest {
  name: string;
  siteId: string;
}

export interface CreateBlastPlanResult {
  blastPlanId: string;
}

export interface ApproveBlastPlanRequest {
  approvedBy: string;
}

export interface BlastPlanSummary {
  blastPlanId: string;
  name: string;
  siteId: string;
  status: string;
  createdUtc: string;
  approvedUtc?: string | null;
}
