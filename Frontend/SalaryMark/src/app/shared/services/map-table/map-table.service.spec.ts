import { TestBed } from '@angular/core/testing';

import { MapTableService } from './map-table.service';

describe('MapTableService', () => {
  let service: MapTableService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MapTableService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
