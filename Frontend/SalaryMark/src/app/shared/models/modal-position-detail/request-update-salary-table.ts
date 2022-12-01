export interface SalaryTableMapping {
  tableId: number;
  unitId: number;
  gsm: number;
  deleted: boolean;
  created: boolean;
}

export interface SalaryTableMappingRequest {
  moduleId: number;
  salaryTableMappings: SalaryTableMapping[];
}
