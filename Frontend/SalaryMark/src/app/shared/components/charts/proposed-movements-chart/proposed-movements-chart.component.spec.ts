import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ProposedMovementsChartComponent } from './proposed-movements-chart.component';

describe('ProposedMovementsChartComponent', () => {
  let component: ProposedMovementsChartComponent;
  let fixture: ComponentFixture<ProposedMovementsChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ProposedMovementsChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProposedMovementsChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
