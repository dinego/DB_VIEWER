import { Injectable } from "@angular/core";

import { HttpClient } from "@angular/common/http";

import {
  IDefault,
  IContractTypesResponse,
  IHoursBaseResponseResponse,
  IGenerateKeyRequest,
  IShareLinkByEmailRequest,
  ITableSalaryResponse,
  IUnit,
  IDisplay,
  ContractTypeEnum,
} from "@/shared/interfaces/positions";
import { environment } from "src/environments/environment";
import { Observable, of } from "rxjs";
import { ProfilesResponse } from "@/shared/models/salary-table";
import {
  ICompanyCombo,
  IParameterGroups,
  IParameters,
} from "@/shared/interfaces/parameters";

@Injectable({
  providedIn: "root",
})
export class CommonService {
  public isPJ?: boolean = null;
  public hourlyBasis?: boolean = null;
  constructor(private http: HttpClient) {}

  getAllUnits(): Observable<IUnit[]> {
    return this.http.get<IUnit[]>(environment.api.common.units);
  }

  getMovements(): Observable<IDefault[]> {
    return this.http.get<IDefault[]>(environment.api.common.movements);
  }

  getDisplayBy(): Observable<IDisplay[]> {
    return this.http.get<IDisplay[]>(environment.api.position.getAllDisplayBy);
  }

  getOccupantList(): Observable<IContractTypesResponse> {
    return this.http.get<IContractTypesResponse>(
      environment.api.common.contractTypes
    );
  }

  getHourlyBasis(): Observable<IHoursBaseResponseResponse> {
    return this.http.get<IHoursBaseResponseResponse>(
      environment.api.common.hourlyBase
    );
  }

  getAllSalaryTables(unitId?: string): Observable<ITableSalaryResponse> {
    let params = "";
    params += unitId ? `unitId=${unitId}&` : "";

    return this.http.get<ITableSalaryResponse>(
      `${environment.api.common.getAllSalaryTables}?${params}`
    );
  }

  getAllProfiles(
    tableId?: number,
    unitId?: number
  ): Observable<ProfilesResponse> {
    let params = "";
    params += tableId ? `tableId=${tableId}&` : "";
    params += unitId ? `unitId=${unitId}` : "";
    return this.http.get<ProfilesResponse>(
      `${environment.api.common.profile}?${params}`
    );
  }

  getShareKey(req: IGenerateKeyRequest): Observable<string> {
    return this.http.post<string>(environment.api.share.generateKeySave, req);
  }

  shareLink(req: IShareLinkByEmailRequest): Observable<any> {
    return this.http.post(environment.api.share.shareLink, req);
  }

  async canAccesHourlyBasis() {
    if (this.hourlyBasis == null)
      this.hourlyBasis = await this.http
        .get<boolean>(environment.api.common.canAccessHoursBase)
        .toPromise();

    return this.hourlyBasis;
  }

  async hidePjSettings() {
    if (this.isPJ != null) return this.isPJ;
    const contractTypes = await this.getOccupantList().toPromise();
    const isCLT =
      contractTypes &&
      contractTypes.contractTypesResponse &&
      contractTypes.contractTypesResponse.length === 1 &&
      contractTypes.contractTypesResponse.some(
        (res) => +res.id === ContractTypeEnum.CLT
      );
    this.isPJ = !isCLT;
    return this.isPJ;
  }

  getUnitsByFilter(tableId: number, groupId?: number): Observable<IUnit[]> {
    let params = "";
    params += tableId ? `tableId=${tableId}&` : "";
    params += groupId ? `groupId=${groupId}` : "";
    return this.http.get<IUnit[]>(
      `${environment.api.common.unitsByFilter}?${params}`
    );
  }

  getAllParameters() {
    return this.http.get<IParameterGroups[]>(
      `${environment.api.common.getAllParameters}`
    );
  }

  getAllCareerAxis(parameters: number[]) {
    return this.http.get<IParameters[]>(
      `${environment.api.common.getAllCareerAxis}?parameters=${JSON.stringify(
        parameters
      )}`
    );
  }

  getAllLevels() {
    return this.http.get<ICompanyCombo[]>(
      `${environment.api.common.getAllLevels}`
    );
  }

  getGsmList(tableId: number, unitId?: number) {
    let params = `tableId=${tableId}&`;
    params += unitId ? `unitId=${unitId}` : "";
    return this.http.get<IDefault[]>(
      `${environment.api.common.getGsmBySalaryTable}?${params}`
    );
  }
}
