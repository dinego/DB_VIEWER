import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { IReports } from "@/shared/interfaces/my-reports";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class ReportService {
  constructor(private http: HttpClient) {}

  downloadFile(reportId: string): Observable<any> {
    return this.http.post(
      environment.api.report.downloadFile,
      {
        reportId,
      },
      {
        responseType: "blob" as "json",
      }
    );
  }

  getReports(
    orderBy: number,
    page: number,
    unitId: number,
    term: string
  ): Observable<IReports[]> {
    const params = new HttpParams({
      fromObject: {
        orderType: `${orderBy}`,
        page: `${page}`,
        unitId: `${unitId}`,
        term,
      },
    });

    return this.http.get<IReports[]>(environment.api.report.getReports, {
      params,
    });
  }

  registerMyReportLog(reportId: number) {
    const body = { reportId: reportId };
    return this.http.post(environment.api.report.registerLog, body);
  }
}
