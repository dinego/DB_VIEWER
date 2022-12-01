export interface Header {
  colPos: number;
  colName: string;
  nickName: string;
  editable: boolean;
  isChecked: boolean;
  disabled: boolean;
  visible: boolean;
  columnId: number;
  sortable: boolean;
}

export interface CheckItem {
  id: number;
  checked: boolean;
}

export interface ListPositions {
  name: string;
  tooltip: string;
  type: number;
}

export interface TooltipBody {
  position: string;
  amount: number;
  occupantType: string;
  occupantTypeId?: number;
  occupantCLT: boolean;
  occupantPJ: boolean;
}

export interface RowBody {
  colPos: number;
  value: string | number;
  subItems?: Array<ListPositions>;
  occupantCLT: boolean;
  occupantPJ: boolean;
  tooltips?: Array<TooltipBody>;
}

export interface PositionBody {
  value: string;
  positionSMId: number;
  tooltips?: Array<TooltipBody>;
}

export interface RowMapBody {
  colPos: number;
  type: string;
  positions: Array<PositionBody>;
}

export interface Body {
  [x: string]: any;
}

export interface Table {
  header: Array<Header>;
  body: Array<Body>;
}
