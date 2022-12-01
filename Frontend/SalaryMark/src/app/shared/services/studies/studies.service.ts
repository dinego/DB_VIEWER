import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { UserService } from "../user/user.service";
import { OrderTypeENUM } from "./common/OrderTypeEnum";
import { Publications, Report, StudyShared } from "./common/publications";

@Injectable({
  providedIn: "root",
})
export class StudiesService {
  constructor(
    private httpClient: HttpClient,
    private userService: UserService
  ) {}

  download(url: string) {
    return this.httpClient.get(url, {
      responseType: "blob" as "json",
    });
  }

  getPublications(
    page: number,
    pageSize: number,
    order: OrderTypeENUM,
    checkBoxes: string,
    term?: string
  ) {
    const params = new HttpParams({
      fromObject: {
        page: page.toString(),
        pageSize: pageSize.toString(),
        term: term ? term.toString() : "",
        orderType: order.toString(),
        studiesType: checkBoxes ? checkBoxes : "",
      },
    });

    return this.httpClient.get<Publications[]>(
      environment.api.studies.getPublications,
      {
        params,
      }
    );
  }

  downloadPdfByPositionId(positionId: number) {
    const url = `${environment.api.studies.getUrlFile}?positionId=${positionId}`;

    this.download(url).subscribe((res: any) => {
      const file = new Blob([res], {
        type: res.type,
      });
      const blob = window.URL.createObjectURL(file);
      window.open(blob);
      window.URL.revokeObjectURL(blob);
    });
  }

  downloadFileByReportId(pub: Publications | Report) {
    const url = `${environment.api.studies.getMyReportsFile}?reportId=${pub.id}`;

    this.download(url).subscribe((res: any) => {
      const file = new Blob([res], {
        type: res.type,
      });
      const blob = window.URL.createObjectURL(file);
      let anchor = document.createElement("a");
      anchor.download = pub.fileName;
      anchor.href = blob;
      anchor.click();
    });
  }

  downloadFileByStudyId(pub: Publications) {
    const url = `${environment.api.studies.getPublicationsFile}?studyId=${pub.id}`;

    this.download(url).subscribe((res: any) => {
      const file = new Blob([res], {
        type: res.type,
      });
      const blob = window.URL.createObjectURL(file);
      let anchor = document.createElement("a");
      anchor.download = pub.fileName;
      anchor.href = blob;
      anchor.click();
    });
  }

  downloadExportExcel(
    url: string,
    panels: number[],
    levels: number[],
    measure?: number
  ) {
    const params = {
      panels: panels,
      levels: levels,
      measure: measure,
    };

    this.httpClient
      .post(url, params, { responseType: "blob" as "json" })
      .subscribe((res: any) => {
        if (res.size <= 0) return;
        const file = new Blob([res], {
          type: res.type,
        });
        const blob = window.URL.createObjectURL(file);

        let anchor = document.createElement("a");
        anchor.download = this.userService.user.company;
        anchor.href = blob;
        anchor.click();
      });
  }

  downloadExportSalaryExcel(
    url: string,
    panels: number[],
    companies: number[],
    levels: number[],
    measure?: number
  ) {
    const params = {
      panels,
      companies,
      levels,
      measure,
    };

    this.httpClient
      .post(url, params, { responseType: "blob" as "json" })
      .subscribe((res: any) => {
        if (res.size <= 0) return;

        const file = new Blob([res], {
          type: res.type,
        });
        const blob = window.URL.createObjectURL(file);

        let anchor = document.createElement("a");
        anchor.download = this.userService.user.company;
        anchor.href = blob;
        anchor.click();
      });
  }

  getStudyShared(secretKey: string): Observable<StudyShared> {
    const params = new HttpParams({
      fromObject: {
        secretKey,
      },
    });

    return this.httpClient.get<StudyShared>(
      environment.api.studies.getStudyShared,
      {
        params,
      }
    ) as Observable<StudyShared>;
  }
}
