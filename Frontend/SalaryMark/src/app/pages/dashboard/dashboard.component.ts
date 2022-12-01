import { Component, OnInit } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

import locales from "@/locales/dashboard";
import { DashboardService } from "@/shared/services/dashboard/dashboard.service";
import { IDefault, IUnit } from "@/shared/interfaces/positions";
import {
  FinancialImpactCards,
  PositionsChart,
  ProposedMovementsTypes,
  ProposedMovementsChart,
  DistributionAnalysisChart,
  ComparativeAnalysisChart,
} from "@/shared/models/dashboard";
import { CommonService } from "@/shared/services/common/common.service";
import { PositioningService } from "@/shared/services/positioning/positioning.service";

@Component({
  selector: "app-dashboard",
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent implements OnInit {
  public comparativeAnalysisChart: ComparativeAnalysisChart;
  public distributionAnalysisChart: DistributionAnalysisChart;
  public financialImpactCards: Array<FinancialImpactCards> = [];
  public locales = locales;
  public movements;
  public movementsList: Array<IDefault> = [];
  public filterByList: IDefault[] = [];
  public filterBySelected: IDefault;
  public positionFilterSelected: IDefault;
  public positionFilterList: IDefault[] = [];
  public positionChart: PositionsChart;
  public proposedMovements: number;
  public proposedMovementsTypes: Array<ProposedMovementsTypes> = [];
  public proposedMovementsChart: ProposedMovementsChart;
  public unit = 0;
  public units: Array<IUnit> = [];
  public comparativeAverage: any;
  public widthModalChart: number;

  constructor(
    private commonService: CommonService,
    private dashboardService: DashboardService,
    private positioningService: PositioningService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.getAllUnits();
    await this.getMovements();
    //await this.getFilterBy();

    this.filterBySelected = {
      id: "10",
      title: "TesteMock",
    };

    this.positionFilterSelected = this.filterBySelected;

    this.getPositionFilter();
    this.loadCards();

    this.ngxSpinnerService.hide();
  }

  async loadCards() {
    await this.getFinancialImpactCards(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getPositionsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getProposedMovementsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements,
      this.proposedMovements
    );
    await this.getDistributionAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getComparativeAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.setSizeModalChart();

    this.ngxSpinnerService.hide();
  }

  async getPositionFilter() {
    this.dashboardService
      .getDisplayFilter(this.filterBySelected.id)
      .toPromise()
      .then((filters) => {
        this.positionFilterList = filters;
        this.positionFilterSelected = this.positionFilterList[0];
      });
    this.ngxSpinnerService.hide();
  }

  async getFilterBy() {
    this.positioningService
      .getDisplayBy()
      .toPromise()
      .then((displayBy) => {
        this.filterByList = displayBy;
        this.filterBySelected = this.filterByList[0];

        this.getPositionFilter();
        this.loadCards();
      });
  }

  async setSizeModalChart() {
    this.widthModalChart = window.innerWidth - window.innerWidth * 0.19;
  }

  async changeUnitSelected(item: IUnit) {
    this.unit = +item.unitId;
    await this.getPositionsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit
    );
    await this.getFinancialImpactCards(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getComparativeAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getDistributionAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getProposedMovementsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements,
      this.proposedMovements
    );
    this.ngxSpinnerService.hide();
  }

  async changeMovements(item: IDefault) {
    this.movements = +item.id;
    await this.getPositionsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit
    );
    await this.getFinancialImpactCards(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getComparativeAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getDistributionAnalysisChart(
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    await this.getProposedMovementsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements,
      this.proposedMovements
    );
    this.ngxSpinnerService.hide();
  }

  async changeProposedMovementsTypes(item: ProposedMovementsTypes) {
    this.proposedMovements = +item.id;
    await this.getProposedMovementsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements,
      this.proposedMovements
    );
    this.ngxSpinnerService.hide();
  }

  async getAllUnits() {
    this.units = await this.commonService.getAllUnits().toPromise();
    this.addDefaultItemUnit();
    this.unit = this.units && this.units.length > 0 ? this.units[0].unitId : 0;
  }

  addDefaultItemUnit() {
    if (this.units && this.units.length > 1) {
      const item: IUnit = {
        unitId: 0,
        unit: "Todas",
      };
      this.units.unshift(item);
    }
  }

  async getMovements() {
    this.movementsList = await this.commonService.getMovements().toPromise();
    this.movements = this.movementsList ? this.movementsList[0].id : null;
  }

  async getFinancialImpactCards(
    displayBy: string,
    unit?: number,
    movements?: number
  ) {
    this.financialImpactCards = await this.dashboardService
      .getFinancialImpactCards(displayBy, unit, movements)
      .toPromise();
  }

  async getDistributionAnalysisChart(
    filterBy: string,
    unit?: number,
    movements?: number
  ) {
    this.distributionAnalysisChart = await this.dashboardService
      .getDistributionAnalysisChart(filterBy, unit, movements)
      .toPromise();
  }

  async getComparativeAnalysisChart(
    filterBy: string,
    unit?: number,
    movements?: number
  ) {
    this.comparativeAnalysisChart = await this.dashboardService
      .getComparativeAnalysisChart(filterBy, unit, movements)
      .toPromise();

    this.comparativeAverage = this.comparativeAnalysisChart;
  }

  async getPositionsChart(
    positionFilter: string,
    filterBy: string,
    unit?: number,
    movements?: number
  ) {
    this.positionChart = await this.dashboardService
      .getPositionsChart(positionFilter, filterBy, unit, movements)
      .toPromise();
  }

  async getProposedMovementsChart(
    positionFilter: string,
    filterBy: string,
    unit?: number,
    movements?: number,
    proposedMovements?: number
  ) {
    this.proposedMovementsChart = await this.dashboardService
      .getProposedMovementsChart(
        positionFilter,
        filterBy,
        unit,
        movements,
        proposedMovements
      )
      .toPromise();
  }

  setBackgroundColorPercent(percent: number) {
    if (percent < 80) {
      return {
        background: "#DEDEDE",
        color: "#1f1f1f",
      };
    } else if (percent >= 80 && percent <= 100) {
      return {
        background: "#CFDDD1",
        color: "#1f1f1f",
      };
    } else if (percent >= 101 && percent <= 120) {
      return {
        background: "#7FA787",
        color: "#ffffff",
      };
    } else {
      return {
        background: "#DEDEDE",
        color: "#1f1f1f",
      };
    }
  }

  async changePosition(e: IDefault) {
    this.positionFilterSelected = e;
    await this.getPositionsChart(
      this.positionFilterSelected.id,
      this.filterBySelected.id,
      this.unit,
      this.movements
    );
    this.ngxSpinnerService.hide();
  }

  filterByEvent(event) {
    this.filterBySelected = event;
    this.getPositionFilter();
    this.loadCards();
    this.ngxSpinnerService.hide();
  }
}
