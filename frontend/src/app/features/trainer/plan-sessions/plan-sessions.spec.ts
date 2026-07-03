import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlanSessions } from './plan-sessions';

describe('PlanSessions', () => {
  let component: PlanSessions;
  let fixture: ComponentFixture<PlanSessions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PlanSessions]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PlanSessions);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
