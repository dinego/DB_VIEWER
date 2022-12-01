import { PJSetting, PJSettingsToSave } from '@/shared/models/pj-setting';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PjSettingsService {
  constructor(private httpClient: HttpClient) {}

  getPJSettings(contractTypeId: number): Observable<PJSetting> {
    const params = new HttpParams({
      fromObject: { contractTypeId: contractTypeId.toString() },
    });

    return this.httpClient.get<PJSetting>(
      environment.api.pjSettings.getPJSettings,
      { params }
    );
  }

  updatePJSetting(pjSettingToSave: PJSettingsToSave): Observable<any> {
    return this.httpClient.put<any>(
      environment.api.pjSettings.updateSettingsPj,
      pjSettingToSave
    );
  }
}
