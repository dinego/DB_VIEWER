import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ComparativeAnalysisChartDashboardComponent } from './comparative-analysis-chart-dashboard.component';

describe('ComparativeAnalysisChartDashboardComponent', () => {
  let component: ComparativeAnalysisChartDashboardComponent;
  let fixture: ComponentFixture<ComparativeAnalysisChartDashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ComparativeAnalysisChartDashboardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ComparativeAnalysisChartDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
