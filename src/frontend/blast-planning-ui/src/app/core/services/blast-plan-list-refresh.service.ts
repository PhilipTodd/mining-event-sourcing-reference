import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BlastPlanListRefreshService {
  private readonly refreshRequestedSubject = new Subject<void>();

  readonly refreshRequested$ =
    this.refreshRequestedSubject.asObservable();

  requestRefresh(): void {
    this.refreshRequestedSubject.next();
  }
}