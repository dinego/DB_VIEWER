import { TestBed } from '@angular/core/testing';

import { HourlyBasisService } from './hourly-basis.service';

describe('HourlyBasisService', () => {
  let service: HourlyBasisService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(HourlyBasisService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
