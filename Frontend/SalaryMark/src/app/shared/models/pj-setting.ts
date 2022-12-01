export interface PJSetting {
  contractTypePercentageTotal: number;
  pjSettingsPercentageTotal: number;
  items: Item[];
}

export interface Item {
  name: string;
  itemTypeId: number;
  pjSettingsId: number;
  contractTypePercentage: number;
  pjSettingsPercentage: number;
  subItems: SubItem[];
}

export interface SubItem {
  name: string;
  contractTypePercentage: number;
}

export interface PJSettingsToSave {
  contractTypeId?: number;
  data: Data[];
}

export interface Data {
  pjSettingsId: number;
  percentage: number;
  itemTypeId: number;
}

export interface PjAccess{
  canAccess: boolean;
}
