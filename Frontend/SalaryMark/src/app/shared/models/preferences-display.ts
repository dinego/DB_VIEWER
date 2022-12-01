import { IDefault } from "../interfaces/positions";

export class PreferenceDisplay {
  globalLabels: GlobalLabels[];
}

export class GlobalLabels {
  id: number;
  name: string;
  alias: string;
  isChecked: boolean;
  isDefault: boolean;
  disabled: boolean;
}
