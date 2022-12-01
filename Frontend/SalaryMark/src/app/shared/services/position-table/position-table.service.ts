import { Observable } from "rxjs";

import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

import {
  ContractTypeEnum,
  IPositionsResponse,
  IUpdateDisplayColumnsListRequest,
} from "@/shared/interfaces/positions";

import { environment } from "src/environments/environment";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { numberFormat } from "highcharts";

@Injectable({
  providedIn: "root",
})
export class PositionListService {
  constructor(private http: HttpClient) {}

  getPositionListTable(
    tableId: string,
    showJustWithOccupants: boolean,
    contractType: string,
    hoursType: string,
    term: string,
    unitId?: number,
    profileId?: number,
    sortColumnId?: number,
    page?: number,
    pageSize?: number,
    isAsc?: boolean,
    columns?: number[],
    columnSearchId?: string,
    filterSearch?: string
  ): Observable<IPositionsResponse> {
    const pageNumber = page || 1;
    const params = new HttpParams({
      fromObject: {
        tableId: tableId.toString(),
        unitId: unitId ? unitId.toString() : "",
        profileId: profileId ? profileId.toString() : "",
        contractType: contractType.toString(),
        hoursType: hoursType.toString(),
        showJustWithOccupants: showJustWithOccupants.toString(),
        page: `${pageNumber}`,
        pageSize: `${pageSize}`,
        term: term,
        sortColumnId: sortColumnId ? sortColumnId.toString() : "",
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
        columnSearchId: columnSearchId ? columnSearchId : "",
        filterSearch: filterSearch ? filterSearch : "",
      },
    });
    return this.http.get<IPositionsResponse>(
      environment.api.position.getAllPositions,
      {
        params,
      }
    );
  }

  getExportExcel(
    tableId: string,
    showJustWithOccupants: boolean,
    contractType: string,
    hoursType: string,
    term: string,
    unitId?: number,
    sortColumnId?: number,
    isAsc?: boolean,
    columns?: number[]
  ): Observable<any> {
    let params = new HttpParams({
      fromObject: {
        tableId: tableId.toString(),
        unitId: unitId ? unitId.toString() : "",
        contractType: contractType.toString(),
        hoursType: hoursType.toString(),
        showJustWithOccupants: showJustWithOccupants.toString(),
        term: term,
        sortColumnId: sortColumnId ? sortColumnId.toString() : "",
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
      },
    });
    return this.http.get(environment.api.position.getAllPositionsExcel, {
      params,
      responseType: "blob",
    });
  }

  getAllProfiles(): Observable<any> {
    return this.http.get<any>(environment.api.profile.getAllProfiles);
  }

  setHeaderChecked(req: IUpdateDisplayColumnsListRequest) {
    return this.http.post(
      environment.api.position.updateDisplayColumnsList,
      req
    );
  }

  getPositionsShared(
    secretKey: string,
    showJustWithOccupants: boolean,
    page?: number,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<IPositionsResponse> {
    const params = new HttpParams({
      fromObject: {
        secretKey,
        page: page ? page.toString() : "1",
        showJustWithOccupants: showJustWithOccupants.toString(),
        sortColumnId: sortColumnId ? sortColumnId.toString() : "",
        isAsc: isAsc.toString(),
      },
    });
    return this.http.get<IPositionsResponse>(
      environment.api.position.sharePositions,
      {
        params,
      }
    );
  }

  saveNewPosition(
    position: string,
    positionIdByLibrary: number,
    levelId: number,
    groupId: number,
    parameters: number[]
  ): Observable<string> {
    const body = {
      position: position,
      positionIdByLibrary: positionIdByLibrary,
      levelId: levelId,
      groupId: groupId,
      parameters: parameters,
    };
    return this.http.post<string>(environment.api.position.savePosition, body);
  }
}
