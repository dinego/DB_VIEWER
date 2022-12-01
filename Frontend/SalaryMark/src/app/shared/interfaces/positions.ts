import { Header } from "../models/map-table";
import { Table } from "../models/position-table";
import { IPermissions } from "../models/token";

export interface IDefault {
  id: string;
  title: string;
}

export interface IDisplayTypes {
  id: number;
  title: string;
}

export interface IDisplayListTypes {
  id: number;
  title: string;
  image: string;
}

export interface IDisplay {
  id: string;
  name: string;
}

export interface IDialogShowModal {
  index: number;
  value: string;
  item: Header;
}

export interface IFramework {
  unit: string;
  currentyPosition: string;
  amountEmployees: string;
}

export interface IDialogPosition {
  // localId: string;
  // gsm: string;
  // level: string;
  // profile: string;
  // positionSM: string;
  // hourlyBasis: string;
  // smCode: string;
  // axisCareer: string;
  // setor: string;
  framework: IFramework[];
  headerPosition: HeaderPosition[];
}

export interface IContractTypesResponse {
  contractTypesResponse: IDefault[];
}

export interface IHoursBaseResponseResponse {
  hoursBaseResponse: IDefault[];
}

export interface IDisplayColumnRequest {
  columnId: number;
  name: string;
  isChecked: boolean;
}

export interface IUpdateDisplayColumnsListRequest {
  userId?: number;
  displayColumns: IDisplayColumnRequest[];
}

export interface IGenerateKeyRequest {
  userId?: number;
  moduleId: number;
  moduleSubItemId: number;
  parameters: {};
  columnsExcluded: [{}];
}

export interface IShareLinkByEmailRequest {
  userId?: number;
  to: string;
  url: string;
}

export interface ITableSalaryResponse {
  tableSalaryResponses: ISalaryTableResponse[];
}

export interface ISalaryTableResponse {
  id: string;
  title: string;
  projectId: number;
}

export interface IUnit {
  unitId?: number;
  unit: string;
}

export interface IPositionsResponse {
  table: Table;
  nextPage: number;
  share: ISharePositionsFilter;
}

export interface UpdateColumns {
  columnId: number;
  name: string;
  isChecked: boolean;
}

export interface ISharePositionsFilter {
  user: string;
  date: Date;
  tableId: number;
  table: string;
  contractTypeId: number;
  contractType: string;
  hoursTypeId: number;
  hoursType: string;
  unit: string;
  unitId: number;
  withOccupants: string;
  isWithOccupants: boolean;
  displayBy: string;
  displayById: number;
  group: string;
  groupId: number;
  permissions: IPermissions;
}

export interface HeaderPosition {
  name: string;
  type: string;
  value: any;
  propertyId: HeaderPositionEnum;
  visible: boolean;
  colPos: number;
}

export enum ContractTypeEnum {
  CLT = 1,
  CLT_Flex = 3,
  CLT_Executive = 2,
  PJ = 11,
  ExecutivePJ = 13,
}

export enum DisplayMapPositionEnum {
  AxisCarreira = 1,
  Area = 2,
  Direction = 3,
  Sector = 4,
}

export enum HeaderPositionEnum {
  localId = 100,
  positionSalaryMark = 1,
  smCode = 8,
  gsm = 1000,
  level = 3,
  axisCareer = 101,
  hourBase = 5,
  profile = 2,
  parameter02 = 1004,
}

export enum HeaderApplyStyleTypeEnum {
  Area = "Área",
  ParameterOne = "Parâmetro 01",
  ParameterTwo = "Parâmetro 02",
  ParameterThree = "Parâmetro 03",
}
