import {
  IDisplayListTypes,
  IDisplayTypes,
} from "@/shared/interfaces/positions";
import locales from "@/locales/framework";
import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";

const visualization: IDisplayListTypes[] = [
  {
    id: DisplayTypesEnum.VALUES,
    title: locales.visualization.types.values,
    image: DisplayImagesListEnum.VALUES,
  },
  {
    id: DisplayTypesEnum.INTENSITY,
    title: locales.visualization.types.intensity,
    image: DisplayImagesListEnum.BAR,
  },
];

export default visualization;
