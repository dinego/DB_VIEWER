import { TestBed } from '@angular/core/testing';

import { SalaryStrategyService } from './salary-strategy.service';

describe('SalaryStrategyService', () => {
  let service: SalaryStrategyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SalaryStrategyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
