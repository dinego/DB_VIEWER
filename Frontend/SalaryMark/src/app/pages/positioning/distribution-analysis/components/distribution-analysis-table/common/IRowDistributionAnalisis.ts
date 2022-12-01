export interface IRowDistributionAnalisis {
  titleCollapser: string;
  insideValue: number;
  aboveValue: number;
  belowValue: number;
  rowsInside: InsideValues[];
}

export interface InsideValues {
  title: string;
  value: number;
}
