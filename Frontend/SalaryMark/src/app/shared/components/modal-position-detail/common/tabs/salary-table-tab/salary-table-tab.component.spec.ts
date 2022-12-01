import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SalaryTableTabComponent } from './salary-table-tab.component';

describe('SalaryTableTabComponent', () => {
  let component: SalaryTableTabComponent;
  let fixture: ComponentFixture<SalaryTableTabComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SalaryTableTabComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SalaryTableTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
