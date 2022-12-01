import { TestBed } from '@angular/core/testing';

import { SalaryTableService } from './salary-table.service';

describe('SalaryTableService', () => {
  let service: SalaryTableService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SalaryTableService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
