export class FinancialImpactChartInput {
  name = '';
  data: FinancialImpactChartDataInput[] = [];
  constructor(result?: FinancialImpactChartInput) {
    Object.assign(this, result);
  }
}

export class FinancialImpactChartDataInput {
  y: number = null;
  percentage: number = null;
  func: number = null;
  funcPercentage: number = null;
  name = '';
  click: ClickFinancialImpactChartDataInput;
}

export class ClickFinancialImpactChartDataInput {
  unitId: number = null;
  displayBy: number = null;
  scenario: number = null;
  serieId: number = null;
  categoryId: any = null;
  legend: string = null;
}
