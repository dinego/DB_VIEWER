import { TestBed } from '@angular/core/testing';

import { PositionListService } from './position-table.service';

describe('PositionListService', () => {
  let service: PositionListService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PositionListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
