import { IPermissions } from "@/shared/models/token";

export interface SalaryChart {
  maxValue: number;
  chart: Array<SalaryInfo>;
  categories: Array<Categories>;
  share: SharedSalaryInfo;
  rangeMin: number;
  rangeMax: number;
}

export interface SalaryInfo {
  type: number;
  name: string;
  data: Array<DataSalaryInfo>;
}

export interface DataSalaryInfo {
  id: number;
  name: string;
  value: number;
}

export interface Categories {
  id: string;
  name: string;
}

export interface SharedSalaryInfo {
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
