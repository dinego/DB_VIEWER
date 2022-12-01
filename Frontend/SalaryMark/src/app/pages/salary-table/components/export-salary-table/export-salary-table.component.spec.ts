import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExportSalaryTableComponent } from './export-salary-table.component';

describe('ExportSalaryTableComponent', () => {
  let component: ExportSalaryTableComponent;
  let fixture: ComponentFixture<ExportSalaryTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ExportSalaryTableComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ExportSalaryTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
