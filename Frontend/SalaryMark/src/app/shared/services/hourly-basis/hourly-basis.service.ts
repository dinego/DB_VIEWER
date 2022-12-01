import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { HourlyBasis, HourlyBasisToSave } from '@/shared/models/hourly-basis';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class HourlyBasisService {
  constructor(private httpClient: HttpClient) {}

  getHourlyBasis(): Observable<HourlyBasis[]> {
    return this.httpClient.get<HourlyBasis[]>(
      environment.api.hourlyBasis.getHourlyBasis
    );
  }

  saveHourlyBasis(hourlyBasisToSave: HourlyBasisToSave): Observable<any> {
    return this.httpClient.put<any>(
      environment.api.hourlyBasis.saveHourlyBasis,
      hourlyBasisToSave
    );
  }
}
