import { Observable } from "rxjs";
import { of } from "rxjs";
import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

import {
  IDialogPosition,
  IPositionsResponse,
  IUpdateDisplayColumnsListRequest,
} from "@/shared/interfaces/positions";

import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class MapTableService {
  constructor(private http: HttpClient) {}

  getMapPositionListTable(
    displayBy: string,
    term: string,
    tableId: string,
    unitId: string,
    groupId: string,
    removeRowsEmpty: boolean = false,
    showJustWithOccupants: boolean = false,
    pageSize: number,
    page?: number,
    isAsc?: boolean,
    columns?: string[]
  ): Observable<IPositionsResponse> {
    const pageNumber = page || 1;
    const params = new HttpParams({
      fromObject: {
        displayBy,
        term,
        tableId: tableId ? tableId : "",
        unitId: unitId ? unitId : "",
        groupId: groupId ? groupId : "",
        removeRowsEmpty: removeRowsEmpty.toString(),
        showJustWithOccupants: showJustWithOccupants.toString(),
        page: `${pageNumber}`,
        pageSize: `${pageSize}`,
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
      },
    });
    return this.http.get<IPositionsResponse>(
      "https://salarymark-api-homolog.carreira.com.br/api/Position/GetMapPosition",
      //environment.api.position.getMapPosition,
      {
        params,
      }
    );
  }

  getExportExcel(
    displayBy: string,
    term: string,
    tableId: string,
    unitId: string,
    groupId: string,
    removeRowsEmpty: boolean = false,
    showJustWithOccupants: boolean = false,
    isAsc?: boolean,
    columns?: string[]
  ): Observable<any> {
    let params = new HttpParams({
      fromObject: {
        displayBy,
        tableId,
        term,
        unitId,
        groupId,
        removeRowsEmpty: removeRowsEmpty.toString(),
        showJustWithOccupants: showJustWithOccupants.toString(),
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
      },
    });
    return this.http.get(environment.api.position.getMapPositionExcel, {
      params,
      responseType: "blob",
    });
  }

  getFullInfoPosition(positionSmId: number): Observable<IDialogPosition> {
    const params = new HttpParams({
      fromObject: { positionSmId: positionSmId.toString() },
    });
    return this.http.get<IDialogPosition>(
      environment.api.position.getFullInfoPosition,
      {
        params,
      }
    );
  }

  setHeaderChecked(req: IUpdateDisplayColumnsListRequest) {
    return this.http.post(
      environment.api.position.updateDisplayColumnsList,
      req
    );
  }

  setHeaderMapChecked(req: IUpdateDisplayColumnsListRequest) {
    return this.http.post(
      environment.api.position.updateDisplayColumnsMap,
      req
    );
  }

  getMapPositionsShared(
    secretKey: string,
    removeRowsEmpty: boolean,
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
        removeRowsEmpty: removeRowsEmpty.toString(),
        sortColumnId: sortColumnId ? sortColumnId.toString() : "",
        isAsc: isAsc.toString(),
      },
    });
    return this.http.get<IPositionsResponse>(
      environment.api.position.shareMap,
      {
        params,
      }
    );
  }
}
