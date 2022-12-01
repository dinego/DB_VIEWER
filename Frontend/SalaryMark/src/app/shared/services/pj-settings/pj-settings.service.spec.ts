import { TestBed } from '@angular/core/testing';

import { PjSettingsService } from './pj-settings.service';

describe('PjSettingsService', () => {
  let service: PjSettingsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PjSettingsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
