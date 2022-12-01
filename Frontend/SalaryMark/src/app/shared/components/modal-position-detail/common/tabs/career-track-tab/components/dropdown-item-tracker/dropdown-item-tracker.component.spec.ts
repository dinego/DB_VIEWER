import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DropdownItemTrackerComponent } from './dropdown-item-tracker.component';

describe('DropdownItemTrackerComponent', () => {
  let component: DropdownItemTrackerComponent;
  let fixture: ComponentFixture<DropdownItemTrackerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DropdownItemTrackerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DropdownItemTrackerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
