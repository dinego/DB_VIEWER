export interface IBodyData {
  rows: IRowData[];
}

export interface IRowData {
  columns: ICellData[];
}

export interface ICellData {
  colPos: number;
  value: string;
  type: string;
  groupId: number;
  trackId: number;
}

export interface IAlteredCel {
  rowPos: number;
  colPos: number;
  value: string;
  type: string;
  groupId: number;
  trackId: number;
}

export interface ISaveSalaryStrategy {
  table: string;
  tableId: number;
  salaryStrategy: IAlteredCel[];
}
