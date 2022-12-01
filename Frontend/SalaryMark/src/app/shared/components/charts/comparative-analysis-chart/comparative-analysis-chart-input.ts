export class ComparativeAnalysisChartInput {
  chart: { chart: ChartComparativeAnalysis[]; average: number };

  categories: Category[] = [];
  share?: null;
  constructor(result?: ComparativeAnalysisChartInput) {
    Object.assign(this, result);
  }
}

export class ChartComparativeAnalysis {
  name: string;
  type: ComparativeAnalyseEnum;
  data: DataComparativeAnalysis[];
}

export class DataComparativeAnalysis {
  name: string;
  percentage: number;
  click: ClickComparativeAnalyseChartDataInput;
}
export class ClickComparativeAnalyseChartDataInput {
  unitId: number = null;
  displayBy: number = null;
  scenario: number = null;
  categoryId: any = null;
  careerAxis: string;
}

export enum ComparativeAnalyseEnum {
  MidPointToMaximum = 1,
  MidpointToMinimum = 2,
  PeopleFrontMidPoint = 3,
}

export interface FullInfoComparativeAnalysis {
  nextPage: number;
  category: string;
  scenario: string;
  table: Table;
}

export interface Table {
  header: Header[];
  body: Array<Header[]>;
}

export interface Header {
  colPos: number;
  value: string;
  type: string;
  columnId: number;
  editable: boolean;
  isChecked: boolean;
  visible: boolean;
  sortable: boolean;
}

export interface CheckItem {
  id: number;
  checked: boolean;
}

export interface ListPositions {
  name: string;
  tooltip: string;
  type: number;
}

export interface Category {
  name: string;
  id: string;
}
