import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { IDisplayListTypes } from "@/shared/interfaces/positions";

const displayTypesList: IDisplayListTypes[] = [
  {
    id: DisplayTypesEnum.TABLE,
    title: "Tabela",
    image: DisplayImagesListEnum.TABLE,
  },
  {
    id: DisplayTypesEnum.INTENSITY,
    title: "Gráfico",
    image: DisplayImagesListEnum.BAR,
  },
];

export default displayTypesList;
