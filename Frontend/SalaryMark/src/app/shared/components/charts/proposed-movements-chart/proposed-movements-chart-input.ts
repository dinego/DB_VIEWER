import { ProposedMovementsChartType } from './proposed-movements-chart-type';

export class ProposedMovementsChartInput {
  name = '';
  type: ProposedMovementsChartType = ProposedMovementsChartType.AdequacyOfNomenclature;
  data: ProposedMovementsChartInputData[];
  constructor(result?: ProposedMovementsChartInput) {
    Object.assign(this, result);
  }
}

export class ProposedMovementsChartInputData {
  name: string;
  value: number;
  click: ClickProposedMovementsChartDataInput;
}

export class ClickProposedMovementsChartDataInput {
  unitId: number = null;
  displayBy: number = null;
  scenario: number = null;
  categoryId: any = null;
  legend: string = null;
  serieId: number;
}
