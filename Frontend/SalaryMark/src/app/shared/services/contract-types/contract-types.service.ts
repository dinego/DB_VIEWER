import { ContractTypes } from '@/shared/models/contract-types';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ContractTypesService {
  constructor(private httpClient: HttpClient) {}

  getContractAllTypes(): Observable<ContractTypes> {
    return this.httpClient.get<ContractTypes>(
      environment.api.contractTypes.getAllContractTypes
    );
  }

  getContractTypesPjSettings(): Observable<ContractTypes> {
    return this.httpClient.get<ContractTypes>(
      environment.api.contractTypes.getContractTypesPjSettings
    );
  }
}
