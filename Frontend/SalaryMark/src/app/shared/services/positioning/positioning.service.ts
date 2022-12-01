import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { environment } from "src/environments/environment";

import {
  ComparativeAnalysisChartInput,
  FullInfoComparativeAnalysis,
} from "@/shared/components/charts/comparative-analysis-chart/comparative-analysis-chart-input";
import { IDefault } from "@/shared/interfaces/positions";
import {
  ComparativeAnalysisTableInput,
  FinancialImpactChart,
  DistributionAnalysisChart,
  ProposedMovementsChart,
  FinancialImpactTable,
  ProposedMovementsTable,
  ICareerTrackPosition,
} from "@/shared/models/positioning";
import { isUndefined } from "@/shared/common/functions";

@Injectable({
  providedIn: "root",
})
export class PositioningService {
  constructor(private httpClient: HttpClient) {}

  getDisplayBy(): Observable<IDefault[]> {
    return this.httpClient.get<IDefault[]>(
      environment.api.positioning.getDisplayBy
    );
  }

  getComparativeAnalysisTableExportedToExcel(
    displayBy: string,
    scenario: string,
    unitId: string
  ): Observable<any> {
    const params = new HttpParams({
      fromObject: {
        displayBy,
        scenario,
        unitId,
      },
    });
    return this.httpClient.get(
      environment.api.positioning.getComparativeAnalysisTableExcel,
      { params, responseType: "blob" }
    );
  }

  getComparativeAnalysisChartShared(
    secretKey: string
  ): Observable<ComparativeAnalysisChartInput> {
    const params = new HttpParams({
      fromObject: {
        secretKey,
      },
    });

    return this.httpClient.get<ComparativeAnalysisChartInput>(
      environment.api.positioning.getSharedComparativeAnalysisChart,
      {
        params,
      }
    );
  }

  getComparativeAnalysisTableShared(
    secretKey: string
  ): Observable<ComparativeAnalysisTableInput> {
    const params = new HttpParams({
      fromObject: {
        secretKey,
      },
    });
    return this.httpClient.get<ComparativeAnalysisTableInput>(
      environment.api.positioning.getSharedComparativeAnalysisTable,
      {
        params,
      }
    );
  }

  getComparativeAnalysisTable(
    displayBy: string,
    displayByVersus: string,
    scenario: string,
    unitId: string
  ): Observable<ComparativeAnalysisTableInput> {
    const params = new HttpParams({
      fromObject: {
        displayBy,
        displayByVersus,
        scenario,
        unitId,
      },
    });

    return this.httpClient.get<ComparativeAnalysisTableInput>(
      environment.api.positioning.getComparativeAnalysisTable,
      { params }
    );
  }

  getFullInfoComparativeAnalysis(
    page: string,
    pageSize: string,
    categoryId: string,
    careerAxis: string,
    displayBy?: string,
    scenario?: string,
    unitId?: string,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<FullInfoComparativeAnalysis> {
    const params = new HttpParams({
      fromObject: {
        ...JSON.parse(
          JSON.stringify({
            categoryId,
            displayBy,
            scenario,
            unitId,
            page,
            pageSize,
            sortColumnId,
            isAsc,
            careerAxis,
          })
        ),
      },
    });
    return this.httpClient.get<FullInfoComparativeAnalysis>(
      environment.api.positioning.getFullInfoComparativeAnalysis,
      { params }
    );
  }

  getFullInfoComparativeAnalysisShare(
    page: string,
    pageSize: string,
    categoryId: string,
    secretKey: string,
    carrerAxis: string,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<FullInfoComparativeAnalysis> {
    const params = new HttpParams({
      fromObject: {
        ...JSON.parse(
          JSON.stringify({
            categoryId,
            secretKey,
            page,
            pageSize,
            sortColumnId,
            isAsc,
            carrerAxis,
          })
        ),
      },
    });
    return this.httpClient.get<FullInfoComparativeAnalysis>(
      environment.api.positioning.getFullInfoComparativeAnalysisShare,
      { params }
    );
  }

  getFinancialImpact(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ): Observable<FinancialImpactChart> {
    let params = "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += scenario ? `scenario=${scenario}` : "";
    return this.httpClient.get<FinancialImpactChart>(
      `${environment.api.positioning.getFinancialImpact}?${params}`
    );
  }

  getShareFinancialImpact(secretKey: string): Observable<FinancialImpactChart> {
    return this.httpClient.get<FinancialImpactChart>(
      `${environment.api.positioning.getShareFinancialImpact}?secretKey=${secretKey}`
    );
  }

  getShareProposedMovements(
    secretKey: string
  ): Observable<ProposedMovementsChart> {
    return this.httpClient.get<ProposedMovementsChart>(
      `${environment.api.positioning.getShareProposedMovements}?secretKey=${secretKey}`
    );
  }

  getShareDistributionAnalysis(
    secretKey: string
  ): Observable<DistributionAnalysisChart> {
    return this.httpClient.get<DistributionAnalysisChart>(
      `${environment.api.positioning.getShareDistributionAnalysis}?secretKey=${secretKey}`
    );
  }

  getFullInfoPositioningFinancialImpact(
    displayBy: number,
    serieId: number,
    scenario: number,
    unitId: number,
    categoryId: number,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<FinancialImpactTable> {
    let params = "";
    params += serieId ? `serieId=${serieId}&` : "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += categoryId ? `categoryId=${categoryId}&` : "";
    params += scenario ? `scenario=${scenario}&` : "";
    params += sortColumnId ? `sortColumnId=${sortColumnId}&` : "";
    params += !isUndefined(isAsc) ? `isAsc=${isAsc}` : "";
    return this.httpClient.get<FinancialImpactTable>(
      `${environment.api.positioning.getFullInfoPositioningFinancialImpact}?${params}`
    );
  }

  getFullInfoPositioningFinancialImpactShare(
    secretKey: string,
    serieId: number,
    categoryId: number,
    sortColumnId?: number,
    isAsc?: boolean
  ): Observable<FinancialImpactTable> {
    let params = "";
    params += secretKey ? `secretKey=${secretKey}&` : "";
    params += serieId ? `serieId=${serieId}&` : "";
    params += categoryId ? `categoryId=${categoryId}&` : "";
    params += sortColumnId ? `sortColumnId=${sortColumnId}&` : "";
    params += !isUndefined(isAsc) ? `isAsc=${isAsc}` : "";
    return this.httpClient.get<FinancialImpactTable>(
      `${environment.api.positioning.getFullInfoPositioningFinancialImpactShare}?${params}`
    );
  }

  getFullInfoProposedMovements(
    displayBy: number,
    page: number,
    pageSize: number,
    scenario: number,
    unitId: number,
    categoryId: number,
    sortColumnId?: number,
    isAsc?: boolean,
    serieId?: number
  ): Observable<ProposedMovementsTable> {
    let params = "";
    params += page ? `page=${page}&` : "";
    params += pageSize ? `pageSize=${pageSize}&` : "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += categoryId ? `categoryId=${categoryId}&` : "";
    params += scenario ? `scenario=${scenario}&` : "";
    params += sortColumnId ? `sortColumnId=${sortColumnId}&` : "";
    params += !isUndefined(isAsc) ? `isAsc=${isAsc}&` : "";
    params += serieId ? `serieId=${serieId}` : "";
    return this.httpClient.get<ProposedMovementsTable>(
      `${environment.api.positioning.getFullInfoProposedMovements}?${params}`
    );
  }

  getFullInfoProposedMovementsShare(
    page: number,
    pageSize: number,
    categoryId: number,
    secretKey: string,
    sortColumnId?: number,
    isAsc?: boolean,
    serieId?: number
  ): Observable<ProposedMovementsTable> {
    let params = "";
    params += page ? `page=${page}&` : "";
    params += pageSize ? `pageSize=${pageSize}&` : "";
    params += secretKey ? `secretKey=${secretKey}&` : "";
    params += categoryId ? `categoryId=${categoryId}&` : "";
    params += sortColumnId ? `sortColumnId=${sortColumnId}&` : "";
    params += !isUndefined(isAsc) ? `isAsc=${isAsc}&` : "";
    params += serieId ? `serieId=${serieId}` : "";
    return this.httpClient.get<ProposedMovementsTable>(
      `${environment.api.positioning.getFullInfoProposedMovementsShare}?${params}`
    );
  }

  getDistributionAnalysis(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ): Observable<DistributionAnalysisChart> {
    let params = "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += scenario ? `scenario=${scenario}` : "";
    return this.httpClient.get<DistributionAnalysisChart>(
      `${environment.api.positioning.getDistributionAnalysis}?${params}`
    );
  }

  getProposedMovements(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ): Observable<ProposedMovementsChart> {
    let params = "";
    params += displayBy ? `displayBy=${displayBy}&` : "";
    params += unitId ? `unitId=${unitId}&` : "";
    params += scenario ? `scenario=${scenario}` : "";
    return this.httpClient.get<ProposedMovementsChart>(
      `${environment.api.positioning.getProposedMovements}?${params}`
    );
  }

  getComparativeAnalysisChart(
    displayBy: string,
    scenario: string,
    unitId: string
  ): Observable<ComparativeAnalysisChartInput> {
    const params = new HttpParams({
      fromObject: {
        displayBy,
        scenario,
        unitId,
      },
    });

    return this.httpClient.get<ComparativeAnalysisChartInput>(
      environment.api.positioning.getComparativeAnalysisChart,
      { params }
    );
  }

  getCareerTrack(cmCode: number): Observable<ICareerTrackPosition> {
    // TODO Implementar get em endpoint
    // const params = new HttpParams({
    //   fromObject: {
    //     cmCode,
    //   },
    // });

    // return this.httpClient.get<ICareerTrackPosition>(
    //   environment.api.positioning.getComparativeAnalysisChart,
    //   { params }
    // );

    const mockDropPositions: IDefault[] = [
      {
        id: "1",
        title: "Coordenador Mocked",
      },
      {
        id: "2",
        title: "Administrador Mocked",
      },
      {
        id: "3",
        title: "Outro Mocked",
      },
    ];

    const career: ICareerTrackPosition = {
      parameters: [
        {
          parameterId: 1,
          parameterName: "Eixo Carreira",
          parametersInner: [
            {
              parameter: "Adm Pessoal",
              parameterId: 1,
              positionsRelated: [
                {
                  level: 2,
                  isHighlighted: true,
                  isPossibility: false,
                  position: "Coordenador Adm Pessoal Jr",
                  positionId: 1002,
                  isDrop: true,
                  isArrow: true,
                  dropItems: mockDropPositions,
                },
                {
                  level: 1,
                  isHighlighted: true,
                  isPossibility: false,
                  position: "Analista Adm Pessoal Sr",
                  positionId: 1001,
                  isDrop: false,
                },
                {
                  level: 0,
                  isHighlighted: false,
                  isPossibility: false,
                  position: "Analista Adm Pessoal Pl",
                  positionId: 1000,
                  isDrop: false,
                  isFirst: true,
                },
              ],
            },
            {
              parameter: "Generalista",
              parameterId: 2,
              positionsRelated: [
                {
                  level: null,
                  isHighlighted: false,
                  isPossibility: true,
                  position: "Especialista RH",
                  positionId: 1004,
                  isDrop: true,
                  isArrow: true,
                  dropItems: mockDropPositions,
                },
              ],
            },
            {
              parameter: "Relações Trabalhistas",
              parameterId: 3,
              positionsRelated: [
                {
                  level: null,
                  isHighlighted: false,
                  isPossibility: true,
                  position: "Especialista Relações Trabalhistas",
                  positionId: 1005,
                  isDrop: true,
                  isArrow: true,
                  dropItems: mockDropPositions,
                },
              ],
            },
          ],
        },
        {
          parameterId: 2,
          parameterName: "Area",
          parametersInner: [
            {
              parameter: "Recursos Humanos",
              parameterId: 1000,
              positionsRelated: null,
            },
          ],
        },
      ],
    };

    return of(career);
  }
}
