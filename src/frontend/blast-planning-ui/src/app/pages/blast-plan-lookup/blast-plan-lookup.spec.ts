import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlastPlanLookup } from './blast-plan-lookup';

describe('BlastPlanLookup', () => {
  let component: BlastPlanLookup;
  let fixture: ComponentFixture<BlastPlanLookup>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BlastPlanLookup]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BlastPlanLookup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
