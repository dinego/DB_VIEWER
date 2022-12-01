import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DistributionAnalysisChartComponent } from './distribution-analysis-chart.component';

describe('DistributionAnalysisChartComponent', () => {
  let component: DistributionAnalysisChartComponent;
  let fixture: ComponentFixture<DistributionAnalysisChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DistributionAnalysisChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DistributionAnalysisChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
