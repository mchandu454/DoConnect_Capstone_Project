import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminHistoryComponent } from './admin-history';

describe('AdminHistoryComponent', () => {
  let component: AdminHistoryComponent;
  let fixture: ComponentFixture<AdminHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AdminHistoryComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AdminHistory);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
