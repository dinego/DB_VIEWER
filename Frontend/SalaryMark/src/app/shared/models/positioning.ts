import { DistribuitionAnalysisChartInput } from "../components/charts/distribution-analysis-chart/distribuition-analysis-chart-input";
import { IDefault } from "../interfaces/positions";
import { IPermissions } from "./token";

export interface ComparativeAnalysisTableInput {
  tables: TableElement[];
  share?: null;
  categories: Category[];
}

export interface Category {
  name: string;
  id: string;
}

export interface TableElement {
  type: number;
  header: Header[];
  total: Total[];
  body: Array<Body[]>;
}

export interface Body {
  colPos: number;
  amount: number | null;
  percentage: number | null;
  name: null | string;
  categoryId: null | string;
}

export interface Header {
  name: string;
  columnId: number;
  colPos: number;
}

export interface Total {
  colPos: number;
  total: number;
  name: null | string;
}

export interface DataFinancialImpact {
  y: number;
  percentage: number;
  func: number;
  funcPercentage: number;
  name: string;
}

export interface FinancialImpact {
  data: Array<DataFinancialImpact>;
  name: string;
}

export interface ColsFinancialImpact {
  colPos: number;
  value: string;
  type: string;
  sortable: boolean;
  columnId: number;
}

export interface TableFinancial {
  header: Array<ColsFinancialImpact>;
  body: Array<Array<ColsFinancialImpact>>;
}

export interface FinancialImpactTable {
  category: string;
  scenario: string;
  table: TableFinancial;
}

export interface ProposedMovementsTable {
  nextPage: number;
  category: string;
  scenario: string;
  table: TableFinancial;
}

export interface Categories {
  id: string;
  name: string;
}

export interface FinancialImpactChart {
  categories: Array<Categories>;
  chart: Array<FinancialImpact>;
  share: SharedFinancialImpact;
}

export interface CheckShowItem {
  id: string;
  name: string;
  checked: boolean;
}

export interface DataDistributionAnalysis {
  id: number;
  name: string;
  value: number;
}

export interface DistributionAnalysis {
  type: number;
  name: string;
  data: Array<DataDistributionAnalysis>;
}

export interface SharedDistributionAnalysis {
  user: string;
  date: Date;
  unit: string;
  unitId: number;
  displayBy: string;
  displayById: number;
  scenario: string;
  scenarioId: number;
  permissions: IPermissions;
}

export interface DistributionAnalysisChart {
  chart: DistribuitionAnalysisChartInput;
  categories: Array<Categories>;
  share: SharedDistributionAnalysis;
}

export interface ProposedMovementsChart {
  chart: Array<DistributionAnalysis>;
  categories: Array<Categories>;
  share: SharedProposedMovement;
}

export interface SharedFinancialImpact {
  user: string;
  date: Date;
  unit: string;
  unitId: number;
  displayBy: string;
  displayById: number;
  scenario: string;
  scenarioId: number;
  permissions: IPermissions;
}

export interface SharedProposedMovement {
  user: string;
  date: Date;
  unit: string;
  unitId: number;
  displayBy: string;
  displayById: number;
  scenario: string;
  scenarioId: number;
  permissions: IPermissions;
}

export interface ICareerTrackPosition {
  parameters: IParameter[];
}

export interface IPosition {
  positionId: number;
  position: string;
  level: number;
  isHighlighted: boolean;
  isPossibility: boolean;
  isDrop: boolean;
  dropItems?: IDefault[];
  isLine?: boolean;
  isFirst?: boolean;
  isLast?: boolean;
  isArrow?: boolean;
}

export interface IParameter {
  parameterId: number;
  parameterName: string;
  parametersInner: IParameterItem[];
}

export interface IParameterItem {
  parameter: string;
  parameterId: number;
  positionsRelated: IPosition[];
}
