import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Level, SaveLevels } from './../../models/level';
import { environment } from './../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class LevelService {
  constructor(private httpClient: HttpClient) {}

  getLevels(): Observable<Level> {
    return this.httpClient.get<Level>(environment.api.level.getLevels);
  }

  saveLavels(saveLevel: SaveLevels): Observable<any> {
    return this.httpClient.put<SaveLevels>(
      environment.api.level.saveLevels,
      saveLevel
    );
  }
}
