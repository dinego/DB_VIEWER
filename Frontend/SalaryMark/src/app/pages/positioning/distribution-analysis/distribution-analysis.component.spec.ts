import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DistributionAnalysisComponent } from './distribution-analysis.component';

describe('DistributionAnalysisComponent', () => {
  let component: DistributionAnalysisComponent;
  let fixture: ComponentFixture<DistributionAnalysisComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DistributionAnalysisComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DistributionAnalysisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
