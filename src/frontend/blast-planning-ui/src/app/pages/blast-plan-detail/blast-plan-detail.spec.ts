import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlastPlanDetail } from './blast-plan-detail';

describe('BlastPlanDetail', () => {
  let component: BlastPlanDetail;
  let fixture: ComponentFixture<BlastPlanDetail>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BlastPlanDetail]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BlastPlanDetail);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
