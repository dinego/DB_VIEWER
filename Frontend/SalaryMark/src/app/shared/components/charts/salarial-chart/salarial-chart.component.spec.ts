import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalarialChartComponent } from './salarial-chart.component';

describe('SalarialChartComponent', () => {
  let component: SalarialChartComponent;
  let fixture: ComponentFixture<SalarialChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalarialChartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalarialChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
