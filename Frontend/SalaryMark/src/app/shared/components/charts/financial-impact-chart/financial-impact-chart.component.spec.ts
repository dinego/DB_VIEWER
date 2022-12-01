import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FinancialImpactChartComponent } from './financial-impact-chart.component';

describe('FinancialImpactChartComponent', () => {
  let component: FinancialImpactChartComponent;
  let fixture: ComponentFixture<FinancialImpactChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FinancialImpactChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinancialImpactChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
