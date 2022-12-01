import { Pipe, PipeTransform } from "@angular/core";
import { TooltipBody } from "../models/position-table";

@Pipe({
  name: "filterTooltip",
})
export class FilterTooltipPipe implements PipeTransform {
  transform(arr: TooltipBody[], searchValue: string) {
    if (!searchValue) return arr;

    return arr.filter((item) => (item.occupantType.toUpperCase() === searchValue.toUpperCase()));
  }
}
