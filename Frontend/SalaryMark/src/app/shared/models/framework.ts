import { FrameworkFullInfoEnum } from '../enum/framework-full-info-enum';

export interface IDefault {
  id: string;
  title: string;
}

export interface IUnit {
  unitId: number;
  unit: string;
}

export interface Table {
  displayType: number;
  displayName: string;
  header: Array<IHeader>;
  body: Array<IBody>;
}

export interface IFramework {
  headerToShow: Array<IHeader>;
  tables: Array<Table>;
}

export interface IFrameworkPayload {
  framework: IFramework;
  nextPage: number;
  share: IShareFrameworkFilter;
}

export interface IHeader {
  columnId: number;
  colPos: number;
  colName: string;
  editable: boolean;
  isChecked: boolean;
  visible: boolean;
  nickName: string;
  disabled: boolean;
}

export interface IBody {
  [x: string]: any;
}

export interface CheckItem {
  id: number;
  checked: boolean;
}

export interface IPermissions {
  canFilterTypeofContract: boolean;
  canFilterMM: boolean;
  canFilterMI: boolean;
  canFilterOccupants: boolean;
  canDownloadExcel: boolean;
  canRenameColumns: boolean;
  canShare: boolean;
  canEditLevels: boolean;
  canEditSalaryStrategy: boolean;
  canEditHourlyBasis: boolean;
  canEditConfigPJ: boolean;
  canEditUser: boolean;
}

export interface IShareFrameworkFilter {
  user: string;
  date: Date;
  contractTypeId: number;
  contractType: string;
  hoursTypeId: number;
  hoursType: string;
  isMI: boolean;
  isMM: boolean;
  unit: string;
  unitId: number;
  permissions: IPermissions;
  scenarioLabel: string;
}

export interface IFrameworkPositionSM {
  type: string;
  gsm: number;
  compare: number;
  position: string;
}

export interface IDialogFramework {
  headerPosition: IHeaderFrameworkPosition[];
  positionsSMFramework: IFrameworkPositionSM[];
  gsmConfig: IGsmConfig
}

export interface IHeaderFrameworkPosition{
  name: string;
  type: string;
  value: any;
  propertyId: FrameworkFullInfoEnum;
  visible: boolean;
  colPos: number;
}

export interface IGsmConfig{
  name: string;
  visible: boolean;
}
