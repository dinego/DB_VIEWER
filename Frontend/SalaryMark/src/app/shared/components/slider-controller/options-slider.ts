import { LabelType } from "@angular-slider/ngx-slider";

export const optionsSlider = (
  complement: string,
  floor: number = 0,
  ceil: number = 50
) => {
  return {
    floor: floor,
    ceil: ceil,
    translate: (value: number, label: LabelType): string => {
      switch (label) {
        case LabelType.Low:
          return value.toString() + (complement ? complement : "");

        case LabelType.High:
          return value.toString() + (complement ? complement : "");

        default:
          return value.toString() + (complement ? complement : "");
      }
    },
  };
};
