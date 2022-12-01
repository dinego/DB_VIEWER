import { Observable } from "rxjs";

import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

import { environment } from "src/environments/environment";
import { IUpdateDisplayColumnsListRequest } from "@/shared/interfaces/positions";
import { IFrameworkPayload, IDialogFramework } from "@/shared/models/framework";

@Injectable({
  providedIn: "root",
})
export class FrameworkService {
  constructor(private http: HttpClient) {}

  getAllFrameworks(
    isMM: boolean,
    isMI: boolean,
    page: number,
    contractType: string,
    hoursType: string,
    term: string,
    unitId: number | string,
    sortColumnId?: number,
    isAsc?: boolean,
    columns?: number[]
  ): Observable<IFrameworkPayload> {
    const params = new HttpParams({
      fromObject: {
        isMM: `${isMM}`,
        isMI: `${isMI}`,
        page: `${page}`,
        contractType,
        hoursType,
        term,
        unitId: unitId ? `${unitId}` : "",
        sortColumnId: sortColumnId ? `${sortColumnId}` : "",
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
      },
    });
    return this.http.get<IFrameworkPayload>(
      environment.api.positioning.getFramework,
      {
        params,
      }
    );
  }

  getAllFrameworkExcel(
    isMM: boolean,
    isMI: boolean,
    contractType: string,
    hoursType: string,
    term: string,
    unitId: number | string,
    sortColumnId?: number,
    isAsc?: boolean,
    columns?: number[]
  ): Observable<any> {
    const params = new HttpParams({
      fromObject: {
        isMM: `${isMM}`,
        isMI: `${isMI}`,
        contractType,
        hoursType,
        term,
        unitId: unitId ? `${unitId}` : "",
        sortColumnId: sortColumnId ? `${sortColumnId}` : "",
        isAsc: isAsc.toString(),
        columns: columns ? JSON.stringify(columns) : "",
      },
    });
    return this.http.get(environment.api.positioning.getFrameworkExcel, {
      params,
      responseType: "blob",
    });
  }

  setHeaderChecked(req: IUpdateDisplayColumnsListRequest) {
    return this.http.post(
      environment.api.positioning.updateDisplayColumnsFramework,
      req
    );
  }

  getPositionsShared(
    secretKey: string,
    page: number,
    contractType: string,
    hoursType: string,
    unitId: number | string,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<IFrameworkPayload> {
    const params = new HttpParams({
      fromObject: {
        secretKey,
        page: `${page}`,
        contractType,
        hoursType,
        unitId: unitId ? `${unitId}` : "",
        sortColumnId: sortColumnId ? `${sortColumnId}` : "",
        isAsc: isAsc.toString(),
      },
    });
    return this.http.get<IFrameworkPayload>(
      environment.api.positioning.shareFramework,
      {
        params,
      }
    );
  }

  getFullInfoFramework(
    salaryBaseId: number,
    isMM: boolean,
    isMI: boolean,
    contractType: string,
    hoursType: string
  ): Observable<IDialogFramework> {
    const params = new HttpParams({
      fromObject: {
        salaryBaseId: `${salaryBaseId}`,
        isMM: `${isMM}`,
        isMI: `${isMI}`,
        contractType: `${contractType}`,
        hoursType: `${hoursType}`,
      },
    });
    return this.http.get<IDialogFramework>(
      environment.api.positioning.getFullInfoFramework,
      {
        params,
      }
    );
  }

  getFullInfoFrameworkForShare(
    secretKey: string,
    salaryBaseId: number
  ): Observable<IDialogFramework> {
    const params = new HttpParams({
      fromObject: {
        secretKey: `${secretKey}`,
        salaryBaseId: `${salaryBaseId}`,
      },
    });
    return this.http.get<IDialogFramework>(
      environment.api.positioning.getFullInfoFrameworkShare,
      {
        params,
      }
    );
  }
}
