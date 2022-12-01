export interface UserParameter {
  id: number;
  name: string;
  mail: string;
  area: null;
  photo: null;
  active: boolean;
  lastAccess: string;
}

export interface ChangeStatusUserToSave {
  userId: number;
  active: boolean;
}

export interface UserParameterDetail {
  id: number;
  name: string;
  email: string;
  sector: null;
  active: boolean;
  photo: null;
  lastAccess: string;
  userPermissions: UserPermissions;
}

export interface UserPermissions {
  levels: Area[];
  sections: Area[];
  permission: Area[];
  areas: Area[];
  contents: Area[];
}

export interface Area {
  name: string;
  id: number;
  isChecked: boolean;
  subItems?: Area[];
}

export class UserParameterAccess {
  showUsers: boolean;
  showPjSettings: boolean;
  showHourlyBasis: boolean;
}
export class SendLinkAccess {
  to: string;
  url: string;
  message: string;
}
