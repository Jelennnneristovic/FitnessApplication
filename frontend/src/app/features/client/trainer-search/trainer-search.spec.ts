import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainerSearch } from './trainer-search';

describe('TrainerSearch', () => {
  let component: TrainerSearch;
  let fixture: ComponentFixture<TrainerSearch>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrainerSearch]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrainerSearch);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
