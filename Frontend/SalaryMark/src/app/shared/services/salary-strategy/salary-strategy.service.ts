import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { ISalaryStrategyPayload } from "@/shared/models/salary-strategy";
import { environment } from "src/environments/environment";
import {
  IAlteredCel,
  ISaveSalaryStrategy,
} from "@/pages/parameters/salary-strategy/common/body-data";

@Injectable({
  providedIn: "root",
})
export class SalaryStrategyService {
  constructor(private http: HttpClient) {}

  getAllSalaryStrategy(
    tableId: number,
    sortableColumnId?: number,
    isSort?: Boolean
  ): Observable<ISalaryStrategyPayload> {
    const params = new HttpParams({
      fromObject: {
        tableId,
        sortColumnId: sortableColumnId ? sortableColumnId.toString() : "",
        isAsc: `${isSort}`,
      },
    });
    return this.http.get<ISalaryStrategyPayload>(
      environment.api.salaryStrategy.getAllSalaryStrategy,
      {
        params,
      }
    );
  }

  updateRows(salaryStrategy: ISaveSalaryStrategy): Observable<any> {
    return this.http.put<any>(
      environment.api.salaryStrategy.updateSalaryStrategy,
      salaryStrategy
    );
  }
}
