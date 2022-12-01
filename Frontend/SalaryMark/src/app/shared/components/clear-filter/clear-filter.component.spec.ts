import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ClearFilterComponent } from './clear-filter.component';

describe('ClearFilterComponent', () => {
  let component: ClearFilterComponent;
  let fixture: ComponentFixture<ClearFilterComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ClearFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClearFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
