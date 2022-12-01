import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SalaryStrategyComponent } from './salary-strategy.component';

describe('SalaryStrategyComponent', () => {
  let component: SalaryStrategyComponent;
  let fixture: ComponentFixture<SalaryStrategyComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SalaryStrategyComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryStrategyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
