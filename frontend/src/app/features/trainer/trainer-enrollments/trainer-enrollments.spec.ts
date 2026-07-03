import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainerEnrollments } from './trainer-enrollments';

describe('TrainerEnrollments', () => {
  let component: TrainerEnrollments;
  let fixture: ComponentFixture<TrainerEnrollments>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrainerEnrollments]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainerEnrollments);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
