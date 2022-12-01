import defaultFonts from "@/shared/view-models/fonts-charts";
import {
  Component,
  OnInit,
  OnChanges,
  SimpleChanges,
  Input,
  Output,
  EventEmitter,
} from "@angular/core";
import * as Highcharts from "highcharts";
import { ProposedMovementsChartInput } from "./proposed-movements-chart-input";
import { ProposedMovementsChartType } from "./proposed-movements-chart-type";

@Component({
  selector: "app-proposed-movements-chart",
  templateUrl: "./proposed-movements-chart.component.html",
  styleUrls: ["./proposed-movements-chart.component.scss"],
})
export class ProposedMovementsChartComponent implements OnInit, OnChanges {
  chart: Highcharts.Chart;

  Highcharts: typeof Highcharts = Highcharts; // required
  chartConstructor = "chart"; // optional string, defaults to 'chart'
  chartOptions: any;

  updateFlag = false; // optional boolean
  oneToOneFlag = true; // optional boolean, defaults to false
  runOutsideAngular = true; // optional boolean, defaults to false

  chartInstance: Highcharts.Chart;

  @Input()
  height = 440;

  @Input()
  valuesProposedMovementsChartsChart: ProposedMovementsChartInput[];

  @Output()
  clickPoint = new EventEmitter<any>();

  @Input() type: string;

  // tslint:disable-next-line: only-arrow-functions
  chartCallback: Highcharts.ChartCallbackFunction = function () {}; // optional function, defaults to null
  constructor() {
    const self = this;
    // saving chart reference using chart callback
    this.chartCallback = (chart) => {
      self.chart = chart;
    };
  }

  colorsSeries(key: ProposedMovementsChartType) {
    const colors = new Map<ProposedMovementsChartType, string>([
      [ProposedMovementsChartType.AdequacyOfNomenclature, "#5C77A6"],
      [ProposedMovementsChartType.ChangeOfPosition, "#A0527E"],
      [ProposedMovementsChartType.WithoutProposedAdjustment, "#9D9D9D"],
    ]);

    return colors.get(key);
  }

  ngOnChanges(changes: SimpleChanges) {
    for (const propName in changes) {
      if (changes.hasOwnProperty(propName)) {
        switch (propName) {
          case "type": {
            if (!changes.type.firstChange) {
              this.chartOptions.chart.type = this.type;
              this.chart.update(this.chartOptions);
              this.updateFlag = true;
            }
            break;
          }
          case "valuesProposedMovementsChartsChart": {
            if (!changes.valuesProposedMovementsChartsChart.firstChange) {
              const result: ProposedMovementsChartInput[] =
                changes.valuesProposedMovementsChartsChart.currentValue;

              this.chartOptions.series = this.fixSeriesChart(result);

              this.chart.update(this.chartOptions);

              this.updateFlag = true;
            }
            break;
          }
        }
      }
    }
  }

  ngOnInit(): void {
    this.updateChart();
    this.updateFlag = true;
  }

  click(point: any) {
    point.options.click.legend = point.series.name;
    this.clickPoint.emit(point.options.click);
  }

  updateChart() {
    const componentScope = this;

    const linearGradient = {
      x1: 0,
      x2: 0,
      y1: 0,
      y2: 1,
    };

    this.chartOptions = {
      chart: {
        spacingTop: 20,
        spacingBottom: 0,
        type: componentScope.type,
        height: componentScope.height,
        events: {
          load() {
            const chart = this;
            const series = chart.series;
            let points;
            let chosenPoint;

            // tslint:disable-next-line: only-arrow-functions
            series.forEach(function (serie, index) {
              points = serie.points;
              // tslint:disable-next-line: only-arrow-functions
              points.forEach(function (point) {
                chosenPoint = point;

                if (point.y < 4) {
                  chosenPoint.update({
                    dataLabels: {
                      x: 60,
                      color: serie.color,
                      style: {
                        ...defaultFonts.legendLabelsTooltip,
                        fontWeight: "bold",
                      },
                    },
                  });
                }
              });
            });

            chart.redraw(false);
          },
        },
      },
      colors: [
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#6685BE"],
            [1, "#516A98"],
          ],
        },

        {
          linearGradient: linearGradient,
          stops: [
            [0, "#A0527E"],
            [1, "#713557"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#A0A0A0"],
            [1, "#989898"],
          ],
        },
      ],
      credits: {
        enabled: false,
      },
      exporting: { enabled: false },
      title: {
        text: null,
      },
      xAxis: {
        lineColor: "white",
        type: "category",
        labels: {
          style: {
            ...defaultFonts.axiesXY,
          },
        },
      },
      yAxis: {
        labels: {
          enabled: false,
        },
        min: 0,
        gridLineWidth: 0,
        title: {
          text: null,
        },
      },
      legend: {
        symbolHeight: 15,
        symbolWidth: 15,
        symbolRadius: 2,
        squareSymbol: false,
        itemMarginBottom: 7,
        layout: "horizontal",
        useHTML: true,
        itemStyle: {
          ...defaultFonts.legendLabelsTooltip,
        },
      },
      tooltip: {
        enabled: false,
      },
      plotOptions: {
        series: {
          cursor: "pointer",
          events: {
            click(event) {
              componentScope.click(event.point);
            },
            legendItemClick() {
              const amountVisible =
                this.chart.series.filter((s) => s.visible).length -
                (this.visible ? 1 : 0);

              if (amountVisible >= 1) {
                return true;
              }
              return false;
            },
          },
        },
        column: {
          stacking: "normal",
          borderWidth: 0,
          maxPointWidth: 80,
          dataLabels: {
            enabled: true,
            style: {
              ...defaultFonts.legendLabelsTooltip,
              color: "white",
              textOutline: null,
            },
            formatter() {
              return Highcharts.numberFormat(this.point.y, 0) + "%";
            },
          },
        },
        bar: {
          stacking: "normal",
          borderWidth: 0,
          maxPointWidth: 80,
          dataLabels: {
            enabled: true,
            style: {
              ...defaultFonts.legendLabelsTooltip,
              color: "white",
              textOutline: null,
            },
            formatter() {
              return Highcharts.numberFormat(this.point.y, 0) + "%";
            },
          },
        },
      },
      series: componentScope.fixSeriesChart(
        componentScope.valuesProposedMovementsChartsChart
      ),
    };
  }
  fixSeriesChart(value: ProposedMovementsChartInput[]) {
    const result = [];

    value.forEach((f) => {
      result.push({
        name: f.name,
        stack: "stack",
        data: f.data.map((m) => ({ name: m.name, y: m.value, click: m.click })),
        point: {
          events: {
            mouseOver: function (e) {
              this.dataLabel.css({
                fontWeight: "bold",
              });
            },
            mouseOut: function (e) {
              this.dataLabel.css({
                fontWeight: "normal",
              });
            },
          },
        },
      });
    });

    return result;
  }
}
