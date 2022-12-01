import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import {
  FinancialImpactCards,
  PositionsChart,
  ProposedMovementsTypes,
  ProposedMovementsChart,
  DistributionAnalysisChart,
  ComparativeAnalysisChart,
} from "@/shared/models/dashboard";
import { environment } from "src/environments/environment";
import { IDefault } from "@/shared/interfaces/positions";

@Injectable({
  providedIn: "root",
})
export class DashboardService {
  constructor(private http: HttpClient) {}

  getFinancialImpactCards(
    displayBy?: string,
    unit?: number,
    movements?: number
  ): Observable<FinancialImpactCards[]> {
    let params = "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `movements=${movements}&` : "";
    params += displayBy ? `displayBy=${displayBy}` : "";
    return this.http.get<FinancialImpactCards[]>(
      `${environment.api.dashboard.getFinancialImpactCards}?${params}`
    );
  }

  getDistributionAnalysisChart(
    displayBy?: string,
    unit?: number,
    movements?: number
  ): Observable<DistributionAnalysisChart> {
    let params = "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `movements=${movements}&` : "";
    params += displayBy ? `displayBy=${displayBy}` : "";
    return this.http.get<DistributionAnalysisChart>(
      `${environment.api.dashboard.getDistributionAnalysisChart}?${params}`
    );
  }

  getComparativeAnalysisChart(
    displayBy: string,
    unit?: number,
    movements?: number
  ): Observable<ComparativeAnalysisChart> {
    let params = "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `movements=${movements}&` : "";
    return this.http.get<ComparativeAnalysisChart>(
      `${environment.api.dashboard.getComparativeAnalysisDash}?${params}`
    );
  }

  getPositionsChart(
    positionFilter: string,
    displayBy: string,
    unit?: number,
    movements?: number
  ): Observable<PositionsChart> {
    let params = "";
    params += positionFilter ? `positionFilter=${positionFilter}&` : "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `movements=${movements}` : "";

    return this.http.get<PositionsChart>(
      `${environment.api.dashboard.getPositionsChart}?${params}`
    );
  }

  getProposedMovementsChart(
    positionFilter: string,
    displayBy: string,
    unit?: number,
    movements?: number,
    proposedMovements?: number
  ): Observable<ProposedMovementsChart> {
    let params = "";
    params += positionFilter ? `positionFilter=${positionFilter}&` : "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `&movements=${movements}` : "";
    params += proposedMovements ? `proposedMovements=${proposedMovements}` : "";
    return this.http.get<ProposedMovementsChart>(
      `${environment.api.dashboard.getProposedMovementsChart}?${params}`
    );
  }

  getDisplayFilter(
    filterBy: string,
    unit?: number,
    movements?: number
  ): Observable<IDefault[]> {
    let params = "";
    params += unit ? `unit=${unit}&` : "";
    params += movements ? `&movements=${movements}&` : "";
    params += filterBy ? `displayBy=${filterBy}` : "";
    return this.http.get<IDefault[]>(
      `${environment.api.positioning.getDisplayFilter}?${params}`
    );
  }
}
