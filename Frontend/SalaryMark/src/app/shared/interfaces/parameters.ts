export interface IParameter {
  id: string;
  param: string;
  groupName: number;
}

export interface IParameters {
  id: number;
  title: string;
  type?: number;
  parentParameterId?: number;
}

export interface IParameterGroups {
  parameterId: number;
  parameters: IParameters[];
}

export interface ICompanyCombo {
  companyId: number;
}
