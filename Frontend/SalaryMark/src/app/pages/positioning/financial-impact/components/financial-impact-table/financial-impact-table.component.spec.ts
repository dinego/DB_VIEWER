import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FinancialImpactTableComponent } from './financial-impact-table.component';

describe('FinancialImpactTableComponent', () => {
  let component: FinancialImpactTableComponent;
  let fixture: ComponentFixture<FinancialImpactTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FinancialImpactTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FinancialImpactTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
