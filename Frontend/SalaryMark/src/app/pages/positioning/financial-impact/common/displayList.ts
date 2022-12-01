import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { IDisplayListTypes } from "@/shared/interfaces/positions";

const displayTypesList: IDisplayListTypes[] = [
  {
    id: DisplayTypesEnum.BAR,
    title: "Gráfico de Barras",
    image: DisplayImagesListEnum.BAR,
  },
  {
    id: DisplayTypesEnum.COLUMNS,
    title: "Gráfico de Colunas",
    image: DisplayImagesListEnum.COLUMN,
  },
  {
    id: DisplayTypesEnum.TABLE,
    title: "Tabela",
    image: DisplayImagesListEnum.TABLE,
  },
];

export default displayTypesList;
