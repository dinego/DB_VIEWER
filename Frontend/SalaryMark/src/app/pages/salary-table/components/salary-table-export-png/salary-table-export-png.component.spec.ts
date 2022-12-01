import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryTableExportPngComponent } from './salary-table-export-png.component';

describe('SalaryTableExportPngComponent', () => {
  let component: SalaryTableExportPngComponent;
  let fixture: ComponentFixture<SalaryTableExportPngComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryTableExportPngComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryTableExportPngComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
