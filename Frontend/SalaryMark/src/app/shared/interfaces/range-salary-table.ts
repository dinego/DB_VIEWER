export interface IRangeSalaryGraph {
  range: IRange;
  defaultRange: IRange;
}

interface IRange {
  min: number;
  max: number;
}
