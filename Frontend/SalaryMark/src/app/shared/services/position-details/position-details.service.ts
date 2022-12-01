import { IParameter, IParameters } from "@/shared/interfaces/parameters";
import {
  PositionDetail,
  PositionDetailRequest,
} from "@/shared/interfaces/position-detail";
import { SalaryTablePositionDetails } from "@/shared/interfaces/salary-table-position-details";
import { SalaryTableMappingRequest } from "@/shared/models/modal-position-detail/request-update-salary-table";
import { IBodyResponse } from "@/shared/models/modal-position-detail/salary-table-tab";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class PositionDetailsService {
  constructor(private httpClient: HttpClient) {}

  getPositionDetails(
    positionId: number,
    moduleId: number
  ): Observable<PositionDetail> {
    let params = `positionId=${positionId}&`;
    params += `moduleId=${moduleId}`;

    return this.httpClient.get<PositionDetail>(
      `${environment.api.positionDetails.getDetails}?${params}`
    );
  }

  putPositionDetails(positionDetail: PositionDetailRequest): Observable<any> {
    return this.httpClient.put<any>(
      `${environment.api.positionDetails.updatePositionDetail}`,
      positionDetail
    );
  }

  addNewParameter(parameter, parameterParentId): Observable<IParameters> {
    const request = {
      parameter,
      parameterParentId,
    };
    return this.httpClient.post<IParameters>(
      `${environment.api.positionDetails.addNewParameter}`,
      request
    );
  }

  getSalaryTableByGsm(
    tableId: number,
    unitId: number,
    gsm: number,
    contractType: number,
    hoursType: number
  ): Observable<IBodyResponse[]> {
    let params = `tableId=${tableId}`;
    params += `&unitId=${unitId}`;
    params += `&gsm=${gsm}`;
    params += contractType ? `&contractType=${contractType}` : "";
    params += hoursType ? `&hoursType=${hoursType}` : "";

    return this.httpClient.get<IBodyResponse[]>(
      `${environment.api.positionDetails.getSalaryTableValuesByGSM}?${params}`
    );
  }

  getSalaryTable(
    moduleId: number,
    positionId: number,
    tableId: number,
    contractType: number,
    hoursType: number,
    page: number,
    pageSize: number,
    unitId?: number,
    isAsc?: boolean,
    sortColumnId?: number
  ): Observable<SalaryTablePositionDetails> {
    let params = `tableId=${tableId}`;
    params += `&positionId=${positionId}`;
    params += `&moduleId=${moduleId}`;
    params += unitId ? `&unitId=${unitId}` : "";
    params += `&contractType=${contractType}`;
    params += `&hoursType=${hoursType}`;
    params += `&page=${page}`;
    params += `&pageSize=${pageSize}`;
    params += `&isAsc=${isAsc}`;
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";

    return this.httpClient.get<SalaryTablePositionDetails>(
      `${environment.api.positionDetails.getSalaryTableMapping}?${params}`
    );
  }

  updateSalaryTableMapping(
    request: SalaryTableMappingRequest
  ): Observable<any> {
    return this.httpClient.put(
      environment.api.positionDetails.updateSalaryTableMapping,
      request
    );
  }
}
