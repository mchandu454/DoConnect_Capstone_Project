import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminAnswersComponent } from './admin-answers';

describe('AdminAnswersComponent', () => {
  let component: AdminAnswersComponent;
  let fixture: ComponentFixture<AdminAnswersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminAnswersComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminAnswers);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
