export interface HourlyBasis {
  id: number;
  baseSalary: string;
  display: boolean;
  parameters: Parameters;
}

export interface Parameters {
  selectedValue: number | null;
  enabled: boolean;
  options: number[] | null;
}

export interface HourlyBasisToSave {
  hourlyBasis: HourlyBaseToSave[];
}

export interface HourlyBaseToSave {
  id: number;
  display: boolean;
  selectedValue: number;
}

export enum HourlyBasisEnum {
  MonthSalary = 0,
  HourSalary = 1,
  YearSalary = 2
}
