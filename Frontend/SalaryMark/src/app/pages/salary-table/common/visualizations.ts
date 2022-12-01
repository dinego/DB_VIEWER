import { IDisplayListTypes } from "@/shared/interfaces/positions";
import locales from "@/locales/salary-table";
import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";

const visualization: IDisplayListTypes[] = [
  {
    id: DisplayTypesEnum.COLUMNS,
    title: locales.visualization.types.columns,
    image: DisplayImagesListEnum.COLUMN,
  },
  {
    id: DisplayTypesEnum.BAR,
    title: locales.visualization.types.bars,
    image: DisplayImagesListEnum.BAR,
  },
  {
    id: DisplayTypesEnum.TABLE,
    title: locales.visualization.types.table,
    image: DisplayImagesListEnum.TABLE,
  },
];

export default visualization;
