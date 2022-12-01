import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DistributionAnalysisTableComponent } from './distribution-analysis-table.component';

describe('DistributionAnalysisTableComponent', () => {
  let component: DistributionAnalysisTableComponent;
  let fixture: ComponentFixture<DistributionAnalysisTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DistributionAnalysisTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DistributionAnalysisTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
