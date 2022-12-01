import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PositionsDashboardChartComponent } from './positions-dashboard-chart.component';

describe('PositionsDashboardChartComponent', () => {
  let component: PositionsDashboardChartComponent;
  let fixture: ComponentFixture<PositionsDashboardChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PositionsDashboardChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PositionsDashboardChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
