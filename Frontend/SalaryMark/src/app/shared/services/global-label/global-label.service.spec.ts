import { TestBed } from '@angular/core/testing';

import { GlobalLabelService } from './global-label.service';

describe('GlobalLabelService', () => {
  let service: GlobalLabelService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GlobalLabelService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
