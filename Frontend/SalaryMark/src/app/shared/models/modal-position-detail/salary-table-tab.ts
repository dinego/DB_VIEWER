export interface ITable {
  header: IHeader[];
  body: IBody[];
}

export interface IHeader {
  colName: string;
  colPos: number;
  columnId: number;
  disabled: boolean;
  editable: boolean;
  isChecked: boolean;
  isDesc: boolean;
  isMidPoint: boolean;
  nickName: string;
  sortClass?: string;
  sortable: boolean;
  visible: boolean;
}

export interface IBody {
  colPos: number;
  isMidPoint: boolean;
  occupantCLT: boolean;
  occupantPJ: boolean;
  type: string;
  value: string;
}

export interface IBodyResponse {
  colPos: number;
  value: string;
  type: string;
  isMidPoint: boolean;
  occupantCLT: boolean;
  occupantPJ: boolean;
}
