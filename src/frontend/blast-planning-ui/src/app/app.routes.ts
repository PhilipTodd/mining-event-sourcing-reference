import { Routes } from '@angular/router';
import { MsalGuard } from '@azure/msal-angular';

import { Dashboard } from './pages/dashboard/dashboard';
import { CreateBlastPlan } from './pages/create-blast-plan/create-blast-plan';
import { BlastPlanDetail } from './pages/blast-plan-detail/blast-plan-detail';
import { BlastPlanLookup } from './pages/blast-plan-lookup/blast-plan-lookup';
import { About } from './pages/about/about';

export const routes: Routes = [
  {
    path: '',
    component: Dashboard
  },
  {
    path: 'blast-plans/create',
    component: CreateBlastPlan,
    canActivate: [MsalGuard]
  },
  {
    path: 'blast-plans/search',
    component: BlastPlanLookup
  },
  {
    path: 'blast-plans/:id',
    component: BlastPlanDetail
  },
  {
    path: 'about',
    component: About
  },
  {
    path: '**',
    redirectTo: ''
  }
];
