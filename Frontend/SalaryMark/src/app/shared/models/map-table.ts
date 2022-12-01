export interface Header {
  colPos: number;
  colName: string;
  editable: boolean;
  isChecked: boolean;
  nickName?: string;
  visible: boolean;
  disabled: boolean;
  sortable: boolean;
  columnId: number;
}

export interface ListPositions {
  name: string;
  tooltip: string;
  type: number;
}

export interface RowBody {
  colPos: number;
  occupantTypeId?: number;
  value: string | number;
  subItems?: Array<ListPositions>;
}

export interface Body {
  [x: string]: any;
}

export interface Table {
  header: Array<Header>;
  body: Array<Body>;
}
