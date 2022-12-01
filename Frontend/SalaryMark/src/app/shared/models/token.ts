export enum ProductTypeEnum {
  CS = 1,
  SM = 2,
}

export enum AccessTypeEnum {
  Access = 1,
  NotAccess = 2,
  AccessByEmployee = 3,
}
export interface IToken {
  products: IProduct;
  user: IUser;
}

export interface IProduct {
  projectId: number;
  productType: ProductTypeEnum;
  productName: string;
  userCompanies: number[];
  accessType: AccessTypeEnum;
  modules: IModules[];
  permissions: IPermissions;
}
export interface IUser {
  id: number;
  name: string;
  email: string;
  companyId: number;
  company: string;
  key: number;
  simulated: boolean;
  isFirstAccess: boolean;
  isAdmin: boolean;
}

export interface IModules {
  id: number;
  subItems: number[];
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
  canEditGlobalLabels: boolean;
  canEditLocalLabels: boolean;
  inactivePerson: boolean;
  canDisplayEmployeeName: boolean;
  canDisplayBossName: boolean;
  canEditProjectSalaryTablesValues: boolean;
  canChooseDefaultParameter: boolean;
  canMoveNextStep: boolean;
  canAddPosition: boolean;
  canEditPosition: boolean;
  canDeletePosition: boolean;
  canEditListPosition: boolean;
  canEditGSMMappingTable: boolean;
  canEditSalaryTableValues: boolean;
  canAddPeople: boolean;
  canDeletePeople: boolean;
  canEditPeople: boolean;
  canEditMappingPositionSM: boolean;
}

export class Token implements IToken {
  products: IProduct;
  user: IUser;
  constructor(snapshotToken?: IToken) {
    Object.assign(this, snapshotToken);
  }
}

export interface GenerateLinkAccessFromUserParameter {
  userId: number;
}
