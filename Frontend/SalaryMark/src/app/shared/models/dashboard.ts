
export interface FinancialImpactCards {
  employeeAmount: number;
  employeePercentage: number;
  cost: number;
  costPercentage: number;
  analyseFinancialImpactId: number;
  analyseFinancialImpactName: string;
}

export interface PositionsChart {
  name: string;
  percentage: number;
  amountPositions: number;
  occupantsPercentage: number;
}


export interface ProposedMovementsTypes {
  id: number;
  title: string;
}

export interface ProposedMovementsChart {
  percentageEmployees: number;
  amountEmployees: number;
  namePositions: string;
  amountPositions: number;
  percentagePositions: number;
}

export interface Data {
  name: string;
  id: number;
  value: number;
}

export interface Chart {
  name: string;
  type: number;
  data: Array<Data>;
}

export interface ComparativeData {
  percentage: number;
  name: string;
}

export interface ComparativeChart {
  name: string;
  type: number;
  data: Array<ComparativeData>;
}

export interface ComparativeAnalysisChart {
  average: number;
  chart: Array<ComparativeChart>;
}


export interface DistributionAnalysisChart {
  chart: Array<Chart>;
}
