import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminQuestionsComponent } from './admin-questions';

describe('AdminQuestionsComponent', () => {
  let component: AdminQuestionsComponent;
  let fixture: ComponentFixture<AdminQuestionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminQuestionsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminQuestionsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
