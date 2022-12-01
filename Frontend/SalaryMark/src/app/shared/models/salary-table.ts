import { IDefault } from "../interfaces/positions";
import { IUpdateTable } from "./editSalarialTable";
import { IPermissions } from "./token";

export interface Header {
  isMidPoint: boolean;
  columnId: any;
  colPos: number;
  colName: string;
  editable: boolean;
  isChecked: boolean;
  visible: boolean;
  disabled: boolean;
  nickName: string;
  sortable: boolean;
  sortClass: string;
  isDesc: boolean;
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

export interface RowBody {
  colPos: number;
  value: string | number;
  subItems?: Array<ListPositions>;
}

export interface Body {
  isMidPoint: boolean;
  colPos: number;
  value: string | number;
  type: string;
  expanded: boolean;
  isChecked: boolean;
  cmCode: number;
}

export interface BodyPosition {
  row: number;
  isMidPoint: boolean;
  rowSpan?: number;
  colPos: number;
  value: string | number;
  type: string;
  expanded: boolean;
  isChecked: boolean;
  activeRow: string;
  occupantCLT: boolean;
  occupantPJ: boolean;
  occupantPositions: string[];
}

export interface ProfilesResponse {
  profilesResponse: Array<IDefault>;
}
export interface Table {
  header: Array<Header>;
  body: Array<Array<Body>>;
}

export interface TablePosition {
  header: Array<Header>;
  body: Array<Array<BodyPosition>>;
}

export interface SalaryTable {
  table: Table;
  tablePosition: TablePosition;
  nextPage: number;
}

export interface SalaryTableResponse {
  getSalaryTable: SalaryTable;
}

export interface UpdateColumns {
  columnId: number;
  name: string;
  isChecked: boolean;
}

export interface IShareSalaryTableFilter {
  user: string;
  date: Date;
  tableId: number;
  projectId: number;
  table: string;
  groupId: number;
  group: string;
  contractTypeId: number;
  contractType: string;
  hoursTypeId: number;
  hoursType: string;
  permissions: IPermissions;
  unit: string;
  unitId: number;
  viewType: TableSalaryViewEnum;
  showAllGsm: boolean;
  rangeInit: number;
  rangeFinal: number;
}

export interface UpdateColumnsRequest {
  displayColumns: Array<UpdateColumns>;
}

export interface UpdateColumnsPositionsRequest {
  displayColumnsPositions: Array<UpdateColumns>;
}

export interface UpdateTableInfoRequest {
  tableInfo: IUpdateTable;
}

export interface IGetSalaryTableExcelResponse {
  file?: string;
  fileName?: string;
}

export enum TableSalaryViewEnum {
  SalaryTable = 1,
  PositionSalaryTable = 2,
  GraphSalaryTable = 3,
}
