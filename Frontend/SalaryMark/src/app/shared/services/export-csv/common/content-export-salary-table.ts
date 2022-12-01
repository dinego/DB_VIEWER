export interface IExportSalaryTable {
  headers: IHeaderExport[];
  body: IBodyExport[];
  fileName: string;
  table: string;
}

export interface IHeaderExport {
  value: string;
}
export interface IBodyExport {
  value: string;
  isMidPoint: boolean;
}
