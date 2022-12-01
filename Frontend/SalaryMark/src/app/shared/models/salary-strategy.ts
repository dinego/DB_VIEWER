export interface ISalaryStrategyPayload {
  header: Array<IHeader>,
  body: Array<IBody>,
  nextPage: number;
}

export interface IHeader {
  columnId: number;
  colPos: number;
  colName: string;
  editable: boolean;
  isChecked: boolean;
  visible: boolean;
  sortable: boolean;
}

export interface IBody {
  [x: string]: any;
}

export interface CheckItem {
  id: number;
  checked: boolean;
}
