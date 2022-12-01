import { PreferenceDisplay } from "./preferences-display";

export class DisplaySettins {
  displayTypes: DisplayType[];
}

export class DisplayType {
  id: number;
  name: string;
  subItems: DisplayItem[];
}

export class DisplayItem {
  id: number;
  name: string;
  isChecked: boolean;
}

export class DisplayTypesConfigurations {
  displayConfiguration: DisplaySettins;
  preference: PreferenceDisplay;
}

export class DisplayTypeSettings {
  id: number;
  subItems: number[];
}
