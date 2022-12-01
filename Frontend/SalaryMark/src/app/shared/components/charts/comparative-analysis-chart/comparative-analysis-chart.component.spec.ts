import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ComparativeAnalysisChartComponent } from './comparative-analysis-chart.component';

describe('ComparativeAnalysisChartComponent', () => {
  let component: ComparativeAnalysisChartComponent;
  let fixture: ComponentFixture<ComparativeAnalysisChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ComparativeAnalysisChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComparativeAnalysisChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
