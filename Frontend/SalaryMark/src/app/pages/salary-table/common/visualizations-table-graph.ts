import { IDisplayListTypes } from "@/shared/interfaces/positions";
import locales from "@/locales/salary-table";
import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";

const visualizationTableGraph: IDisplayListTypes[] = [
  {
    id: DisplayTypesEnum.TABLE,
    title: locales.visualization.types.table,
    image: DisplayImagesListEnum.TABLE,
  },
  {
    id: DisplayTypesEnum.COLUMNS,
    title: locales.visualization.types.graphic,
    image: DisplayImagesListEnum.COLUMN,
  },
];

export default visualizationTableGraph;
