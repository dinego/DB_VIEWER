import { IGlobalLabel, IGlobalLabelToSave } from '@/shared/models/global-label';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class GlobalLabelService {

  constructor(private httpClient: HttpClient) {}

  get(): Observable<IGlobalLabel[]> {

    return this.httpClient.get<IGlobalLabel[]>(environment.api.globalLabels.getGlobalLabels);
  }

  update(globalLables: IGlobalLabelToSave): Observable<any> {
    return this.httpClient.put<any>(
      environment.api.globalLabels.updateGlobalLabels,
      globalLables
    );
  }
}
