export class PositionsDashboardChartInput {
  name = '';
  percentage: number = null;
  amountPositions: number = null;
  occupantsPercentage: number = null;
  constructor(result?: PositionsDashboardChartInput) {
    Object.assign(this, result);
  }
}

