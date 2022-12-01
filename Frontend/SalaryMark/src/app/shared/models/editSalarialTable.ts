export interface IEditSalarialTable {
  salaryTableValues: ISalaryTableValues;
  headers: IHeadersTableValues[];
  rangeEdit: number[];
}

export interface IHeadersTableValues {
  colPos: number;
  colName: string;
  isMidPoint: boolean;
}

export interface ISalaryTableValues {
  salarialTableName: "string";
  gsmInitial: number;
  gsmFinal: number;
  salaryTableValues: ITableValues[];
  tableUpdate: IUpdateTable;
}

export interface IUpdateTable {
  justify: string;
  gsmInitial: number;
  gsmFinal: number;
  typeMultiply: number;
  multiply: number;
}

export interface ITableValues {
  gsm: number;
  salaryTableLocalId: number;
  minor6?: number;
  minor5?: number;
  minor4?: number;
  minor3?: number;
  minor2?: number;
  minor1?: number;
  mid?: number;
  plus1?: number;
  plus2?: number;
  plus3?: number;
  plus4?: number;
  plus5?: number;
  plus6?: number;
}
