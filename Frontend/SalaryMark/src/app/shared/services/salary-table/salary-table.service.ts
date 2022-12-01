import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";

import {
  IShareSalaryTableFilter,
  SalaryTable,
  UpdateColumnsRequest,
} from "@/shared/models/salary-table";

import { environment } from "src/environments/environment";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { SalaryChartInput } from "@/shared/components/charts/salarial-chart/salarial-chart-input";
import { SalaryChart } from "@/pages/salary-table/common/salary-chart";
import { IShareHeader } from "@/shared/models/share-header";
import { IRangeSalaryGraph } from "@/shared/interfaces/range-salary-table";
import { IExportSalaryTable } from "../export-csv/common/content-export-salary-table";

@Injectable({
  providedIn: "root",
})
export class SalaryTableService {
  constructor(private httpClient: HttpClient) {}

  getExportExcel(
    tableId: string,
    unitId: string,
    groupId: string,
    contractTypeId: string,
    hoursTypeId: string,
    isAsc: string,
    showAllGsm: string,
    sortColumnId: string
  ): Observable<IExportSalaryTable> {
    let params = `tableId=${tableId}`;
    params += unitId ? `&unitId=${unitId}` : "";
    params += groupId ? `&groupId=${groupId}` : "";
    params += `&contractType=${contractTypeId}`;
    params += `&hoursType=${hoursTypeId}`;

    params += `&isAsc=${isAsc}`;
    params += showAllGsm ? `&showAllGsm=${showAllGsm}` : "";
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";
    return this.httpClient.get<IExportSalaryTable>(
      `${environment.api.salaryTable.getSalaryTableExcel}?${params}`
    );
  }

  getSalaryTable(
    tableId: number,
    unitId: number,
    contractType: number,
    hoursType: number,
    page: number,
    pageSize: number,
    groupId?: number,
    isAsc?: boolean,
    filterSearch?: string,
    showAllGsm?: boolean,
    sortColumnId?: number
  ): Observable<SalaryTable> {
    let params = `tableId=${tableId}`;
    params += unitId ? `&unitId=${unitId}` : "";
    params += `&contractType=${contractType}`;
    params += `&hoursType=${hoursType}`;
    params += `&page=${page}`;
    params += `&pageSize=${pageSize}`;
    params += groupId ? `&groupId=${groupId}` : "";
    params += filterSearch ? `&filterSearch=${filterSearch}` : "";
    params += `&isAsc=${isAsc}`;
    params += showAllGsm ? `&showAllGsm=${showAllGsm}` : "";
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";

    return this.httpClient.get<SalaryTable>(
      `${environment.api.salaryTable.getSalaryTable}?${params}`
    );
  }

  getSalaryPositionTable(
    tableId: number,
    unitId: number,
    contractType: number,
    hoursType: number,
    page: number,
    pageSize: number,
    groupId?: number,
    isAsc?: boolean,
    filterSearch?: string,
    sortColumnId?: number,
    ignorePagination?: boolean
  ): Observable<SalaryTable> {
    let params = `tableId=${tableId}`;
    params += unitId && unitId > 0 ? `&unitId=${unitId}` : "";
    params += `&contractType=${contractType}`;
    params += `&hoursType=${hoursType}`;
    params += `&page=${page}`;
    params += `&pageSize=${pageSize}`;
    params += groupId ? `&groupId=${groupId}` : "";
    params += filterSearch ? `&filterSearch=${filterSearch}` : "";
    params += `&isAsc=${isAsc}`;
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";
    params += ignorePagination ? `&ignorePagination=${ignorePagination}` : "";
    return this.httpClient.get<SalaryTable>(
      `${environment.api.salaryTable.getSalaryPositionTable}?${params}`
    );
  }

  getEditSalarialTable(
    tableId: number,
    projectId: number,
    groupId?: number
  ): Observable<IEditSalarialTable> {
    let params = `tableId=${tableId}`;
    params += `&projectId=${projectId}`;
    params += groupId ? `&groupId=${groupId}` : "";
    return this.httpClient.get<IEditSalarialTable>(
      `${environment.api.salaryTable.getEditTableValues}?${params}`
    );
  }

  updateColumns(req: UpdateColumnsRequest): Observable<any> {
    return this.httpClient.post(
      environment.api.salaryTable.updateDisplayColumns,
      req
    );
  }

  updateTableValues(req: any): Observable<any> {
    return this.httpClient.post(
      environment.api.salaryTable.updateSalaryTable,
      req
    );
  }

  updateTableInfo(req: any): Observable<any> {
    return this.httpClient.post(
      environment.api.salaryTable.updateSalaryTableInfo,
      req
    );
  }

  postFile(importFile: File): Observable<any> {
    const formData: FormData = new FormData();
    formData.append("fileKey", importFile, importFile.name);
    return this.httpClient.post(
      environment.api.salaryTable.importExcel,
      formData,
      {
        //headers: headers,
      }
    );
  }

  getAllSalaryTables(unitId: number) {
    const params = new HttpParams({
      fromObject: {
        unitId,
      },
    });

    return this.httpClient.get<any>(environment.api.common.getAllSalaryTables, {
      params,
    });
  }

  getSalaryGraph(
    tableId: number,
    rangeInit: number,
    rangeFinal: number,
    contractType: number,
    hoursType: number,
    unitId?: number,
    groupId?: number
  ): Observable<SalaryChart> {
    let params = "";
    params += `tableId=${tableId}&`;
    params += `rangeInit=${rangeInit}&`;
    params += `rangeFinal=${rangeFinal}&`;
    params += `contractType=${contractType}&`;
    params += hoursType ? `hoursType=${hoursType}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += groupId ? `groupId=${groupId}` : "";

    return this.httpClient.get<SalaryChart>(
      `${environment.api.salaryTable.getSalaryGraph}?${params}`
    );
  }

  getShareData(secretKey: string): Observable<IShareSalaryTableFilter> {
    let params = `secretKey=${secretKey}`;
    return this.httpClient.get<IShareSalaryTableFilter>(
      `${environment.api.salaryTable.getShareData}?${params}`
    );
  }

  getSalaryTableShared(
    secretKey: string,
    tableId: number,
    unitId: number,
    contractType: number,
    hoursType: number,
    page: number,
    pageSize: number,
    groupId?: number,
    isAsc?: boolean,
    showAllGsm?: boolean,
    sortColumnId?: number
  ): Observable<SalaryTable> {
    let params = `secretKey=${secretKey}`;
    params += `&tableId=${tableId}`;
    params += unitId ? `&unitId=${unitId}` : "";
    params += `&contractType=${contractType}`;
    params += `&hoursType=${hoursType}`;
    params += `&page=${page}`;
    params += `&pageSize=${pageSize}`;
    params += groupId ? `&groupId=${groupId}` : "";
    params += `&isAsc=${isAsc}`;
    params += showAllGsm ? `&showAllGsm=${showAllGsm}` : "";
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";
    return this.httpClient.get<SalaryTable>(
      `${environment.api.salaryTable.getShareSalaryTable}?${params}`
    );
  }

  getShareSalaryPositionTable(
    secretKey: string,
    tableId: number,
    unitId: number,
    contractType: number,
    hoursType: number,
    page: number,
    pageSize: number,
    groupId?: number,
    isAsc?: boolean,
    filterSearch?: string,
    sortColumnId?: number
  ): Observable<SalaryTable> {
    let params = `secretKey=${secretKey}`;
    params += `&tableId=${tableId}`;
    params += unitId && unitId > 0 ? `&unitId=${unitId}` : "";
    params += `&contractType=${contractType}`;
    params += `&hoursType=${hoursType}`;
    params += `&page=${page}`;
    params += `&pageSize=${pageSize}`;
    params += groupId ? `&groupId=${groupId}` : "";
    params += filterSearch ? `&filterSearch=${filterSearch}` : "";
    params += `&isAsc=${isAsc}`;
    params += sortColumnId ? `&sortColumnId=${sortColumnId}` : "";
    return this.httpClient.get<SalaryTable>(
      `${environment.api.salaryTable.getShareSalaryPositionTable}?${params}`
    );
  }

  getShareSalaryGraph(
    secretKey: string,
    tableId: number,
    rangeInit: number,
    rangeFinal: number,
    contractType: number,
    hoursType: number,
    unitId?: number,
    profileId?: number
  ): Observable<SalaryChart> {
    let params = `secretKey=${secretKey}`;
    params += `&tableId=${tableId}&`;
    params += `rangeInit=${rangeInit}&`;
    params += `rangeFinal=${rangeFinal}&`;
    params += `contractType=${contractType}&`;
    params += hoursType ? `hoursType=${hoursType}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += profileId ? `profileId=${profileId}` : "";
    return this.httpClient.get<SalaryChart>(
      `${environment.api.salaryTable.getShareSalaryGraph}?${params}`
    );
  }

  getRangeSalaryGraph(tableId): Observable<IRangeSalaryGraph> {
    const params = `&tableId=${tableId}&`;
    return this.httpClient.get<IRangeSalaryGraph>(
      `${environment.api.salaryTable.getRangeSalaryGraph}?${params}`
    );
  }

  prepareShareHeader(shareData: IShareSalaryTableFilter): IShareHeader[] {
    return [
      {
        label: "Nome",
        value: shareData.user,
        type: "string",
      },
      {
        label: "Data",
        value: shareData.date,
        type: "date",
      },
      {
        label: "Mostrar Como",
        value: shareData.hoursType,
        type: "string",
      },
      {
        label: "Tipo de Contrato",
        value: shareData.contractType,
        type: "string",
      },
      {
        label: "Perfil",
        value: shareData.group,
        type: "string",
      },
      {
        label: "Unidade",
        value: shareData.unit,
        type: "string",
      },
    ];
  }
}
