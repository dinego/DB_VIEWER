import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { HourlyBasisComponent } from './hourly-basis.component';

describe('HourlyBasisComponent', () => {
  let component: HourlyBasisComponent;
  let fixture: ComponentFixture<HourlyBasisComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ HourlyBasisComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HourlyBasisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
