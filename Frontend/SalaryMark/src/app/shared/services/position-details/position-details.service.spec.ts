import { TestBed } from '@angular/core/testing';

import { PositionDetailsService } from './position-details.service';

describe('PositionDetailsService', () => {
  let service: PositionDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PositionDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
