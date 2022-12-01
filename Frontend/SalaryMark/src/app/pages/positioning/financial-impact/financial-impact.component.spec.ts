import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FinancialImpactComponent } from './financial-impact.component';

describe('FinancialImpactComponent', () => {
  let component: FinancialImpactComponent;
  let fixture: ComponentFixture<FinancialImpactComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FinancialImpactComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FinancialImpactComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
