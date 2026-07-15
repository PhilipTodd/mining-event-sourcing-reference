import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlastPlanList } from './blast-plan-list';

describe('BlastPlanList', () => {
  let component: BlastPlanList;
  let fixture: ComponentFixture<BlastPlanList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BlastPlanList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BlastPlanList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
