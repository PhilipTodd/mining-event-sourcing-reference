import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateBlastPlan } from './create-blast-plan';

describe('CreateBlastPlan', () => {
  let component: CreateBlastPlan;
  let fixture: ComponentFixture<CreateBlastPlan>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateBlastPlan]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateBlastPlan);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
