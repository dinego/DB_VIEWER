import { OrderTypeENUM } from "@/shared/services/studies/common/OrderTypeEnum";

const orders = [
  {
    title: "Mais Recente",
    type: OrderTypeENUM.DataDes,
  },
  {
    title: "Mais Antigo",
    type: OrderTypeENUM.DataAsc,
  },
  {
    title: "A-Z   ",
    type: OrderTypeENUM.TitleAsc,
  },
  {
    title: "Z-A   ",
    type: OrderTypeENUM.TitleDes,
  },
];

export default orders;
