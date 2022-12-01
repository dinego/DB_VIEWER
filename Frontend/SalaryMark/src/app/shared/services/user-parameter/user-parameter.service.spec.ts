import { TestBed } from '@angular/core/testing';

import { UserParameterService } from './user-parameter.service';

describe('UserParameterService', () => {
  let service: UserParameterService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserParameterService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
