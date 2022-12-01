import { DistribuitionAnalysisType } from './distribuition-analysis-type';

export class DistribuitionAnalysisChartInput {
  drillDown: DistribuitionAnalysisDrillDown[] = [];
  main: DistribuitionAnalysisCategory[] = [];
  constructor(result?: DistribuitionAnalysisChartInput) {
    Object.assign(this, result);
  }
}

export class DistribuitionAnalysisCategory {
  name: string;
  type: DistribuitionAnalysisType;
  data: DistribuitionAnalysisCategoryData[];
}
export class DistribuitionAnalysisCategoryData {
  name: string;
  value: number;
}

export class DistribuitionAnalysisDrillDown {
  itemGrouped: string;
  data: DistribuitionAnalysisDrillDownData[];
}

export class DistribuitionAnalysisDrillDownData {
  value: number;
  type: DistribuitionAnalysisType;
  name: string;
  employeeAmount: number;
}
