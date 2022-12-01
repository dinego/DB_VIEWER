export interface IGlobalLabel {
  id: number;
  name: string;
  alias: string;
  isChecked: boolean;
}

export interface IGlobalLabelToSave{
  globalLabels: IGlobalLabel[];
}
