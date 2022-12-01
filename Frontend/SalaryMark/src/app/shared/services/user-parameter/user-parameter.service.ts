import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import {
  ChangeStatusUserToSave,
  UserParameter,
  UserParameterDetail,
} from '@/shared/models/user-parameter';

@Injectable({
  providedIn: 'root',
})
export class UserParameterService {
  constructor(private httpClient: HttpClient) {}

  getUserParameters(page: number): Observable<UserParameter[]> {
    const params = new HttpParams({
      fromObject: { page: page.toString() },
    });

    return this.httpClient.get<UserParameter[]>(
      environment.api.userParameter.getAll,
      { params }
    );
  }

  changeStatusUser(changeStatusUserToSave: ChangeStatusUserToSave) {
    return this.httpClient.put(environment.api.userParameter.changeStatusUser, {
      ...changeStatusUserToSave,
    });
  }

  getUserInformation(userId: number) {
    const params = new HttpParams({
      fromObject: { userId: userId.toString() },
    });

    return this.httpClient.get<UserParameterDetail>(
      environment.api.userParameter.getUserInformation,
      { params }
    );
  }

  saveParameterDetailToSave(
    userParameterDetailToSave: UserParameterDetail
  ): Observable<any> {
    return this.httpClient.post<any>(
      environment.api.userParameter.saveUserInformation,
      userParameterDetailToSave
    );
  }
}
