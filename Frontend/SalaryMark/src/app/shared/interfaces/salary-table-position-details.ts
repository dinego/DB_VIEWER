export interface SalaryTablePositionDetails {
  table: Table;
  nextPage: number;
}

export interface Table {
  header: Array<Header>;
  body: Array<Array<Body>>;
}

export interface Header {
  isMidPoint: boolean;
  columnId: any;
  colPos: number;
  colName: string;
  nickName: string;
  sortable: boolean;
  sortClass: string;
  isDesc: boolean;
}

export interface Body {
  isMidPoint: boolean;
  colPos: number;
  value: string | number;
  type: string;
  occupantCLT: boolean;
  occupantPJ: boolean;
}
