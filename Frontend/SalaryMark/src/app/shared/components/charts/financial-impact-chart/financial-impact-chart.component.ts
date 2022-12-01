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
import { DisplayTypesEnum } from "../../button-list-visualization/common/typeVisualizationsEnum";
import { FinancialImpactChartInput } from "./financial-impact-chart-input";

// https://www.itsolutionstuff.com/post/angular-8-highcharts-example-tutorialexample.html
// https://www.highcharts.com/blog/tutorials/highcharts-and-angular-7/

@Component({
  selector: "app-financial-impact-chart",
  templateUrl: "./financial-impact-chart.component.html",
  styleUrls: ["./financial-impact-chart.component.scss"],
})
export class FinancialImpactChartComponent implements OnInit, OnChanges {
  constructor() {
    const self = this;
    // saving chart reference using chart callback
    this.chartCallback = (chart) => {
      self.chart = chart;
    };
  }

  chart: Highcharts.Chart;

  Highcharts: typeof Highcharts = Highcharts; // required
  chartConstructor = "chart"; // optional string, defaults to 'chart'
  chartOptions: any;

  updateFlag = false; // optional boolean
  oneToOneFlag = true; // optional boolean, defaults to false
  runOutsideAngular = true; // optional boolean, defaults to false

  chartInstance: Highcharts.Chart;

  // input
  @Input()
  height = 440;
  // tslint:disable-next-line: member-ordering
  @Input()
  valuesFinancialImpact: FinancialImpactChartInput[];
  @Input() className = "";
  @Input() chartType = DisplayTypesEnum.BAR;

  // output
  @Output()
  clickPoint = new EventEmitter<any>();

  public chartTypeStr: string;

  // tslint:disable-next-line: only-arrow-functions
  chartCallback: Highcharts.ChartCallbackFunction = function (chart) {}; // optional function, defaults to null

  ngOnInit(): void {
    this.setTypeChart();
    this.updateChart();
    this.updateFlag = true;
  }

  setTypeChart() {
    this.chartTypeStr =
      this.chartType === DisplayTypesEnum.BAR ? "bar" : "column";
  }

  ngOnChanges(changes: SimpleChanges) {
    for (const propName in changes) {
      if (changes.hasOwnProperty(propName)) {
        switch (propName) {
          case "chartType": {
            if (!changes.chartType.firstChange) {
              this.setTypeChart();

              this.chartOptions.chart.type = this.chartTypeStr;
              this.chart.update(this.chartOptions);

              this.updateFlag = true;
            }
          }
          case "valuesFinancialImpact": {
            if (
              changes.valuesFinancialImpact &&
              !changes.valuesFinancialImpact.firstChange
            ) {
              this.chartOptions.series =
                changes.valuesFinancialImpact.currentValue;

              this.chart.update(this.chartOptions);

              this.updateFlag = true;
            }
            break;
          }
        }
      }
    }
  }

  getMaxValueChart() {
    const mapData: any[] = [];

    this.valuesFinancialImpact.forEach((element) => {
      element.data.map(function (d) {
        mapData.push(d.y);
      });
    });

    const ret = Math.max.apply(
      Math,
      mapData.map(function (o) {
        return o;
      })
    );
    return ret;
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
        type: componentScope.chartTypeStr,
        height: componentScope.height,
        spacingBottom: 0,
        spacingTop: 10,
        spacingRight: 30,
      },
      colors: [
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#91B5A1"],
            [1, "#62A37E"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#F4E699"],
            [1, "#F2CC65"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#839CCC"],
            [1, "#5F7298"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#C15657"],
            [1, "#8E4C4D"],
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
      legend: {
        symbolHeight: 15,
        symbolWidth: 22,
        symbolRadius: 4,
        layout: "horizontal",
        itemMarginBottom: 7,
        useHTML: true,
        labelFormatter() {
          if (this.visible) {
            return "<span>" + this.name + ' <i class="fas fa-eye"></i></span>';
          }
          return (
            "<span>" + this.name + ' <i class="fas fa-eye-slash"></i></span>'
          );
        },
        itemStyle: {
          ...defaultFonts.legendLabelsTooltip,
        },
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
          format: "{value:,.0f}",
          style: {
            ...defaultFonts.axiesXY,
          },
        },
        min: 0,
        gridLineWidth: 0,
        title: {
          text: null,
        },
        tickInterval: Math.round(componentScope.getMaxValueChart() / 3),
      },
      tooltip: {
        hideDelay: 0,
        outside: false,
        shared: false,
        useHTML: true,
        backgroundColor: "#E0E0E0",
        borderRadius: 2,
        borderColor: "#E0E0E0",
        shadow: true,
        borderWidth: 1.5,
        style: {
          ...defaultFonts.legendLabelsTooltip,
        },
        formatter() {
          return `<div style='text-align: center;'><b> ${this.key}</b></div>
            <div style='text-align: center;'> R$ ${Highcharts.numberFormat(
              this.point.y,
              0
            )}
            ( ${Highcharts.numberFormat(this.point.percentage, 1)}  %)</div>
            <div style='text-align: center;'> ${Highcharts.numberFormat(
              this.point.func,
              0
            )}
            func. (${Highcharts.numberFormat(
              this.point.funcPercentage,
              0
            )} %)</div>`;
        },
      },

      plotOptions: {
        column: {
          pointPadding: 0.05,
          borderWidth: 0,
          groupPadding: 0.05,
        },
        bar: {
          pointPadding: 0.05,
          borderWidth: 0,
          groupPadding: 0.05,
        },
        series: {
          cursor: "pointer",
          point: {
            events: {
              mouseOver() {
                this.dataLabel.css({
                  color: "transparent",
                });
              },
              mouseOut() {
                this.dataLabel.css({
                  color: "#595959",
                });
              },
            },
          },
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
          dataLabels: {
            enabled: true,
            useHTML: true,
            style: {
              ...defaultFonts.legendLabelsTooltip,
            },
            formatter() {
              if (componentScope.chartType === DisplayTypesEnum.BAR)
                return `<div class="d-flex"><div style='text-align: center; padding-right: 10px; border-right: 1px solid #595959'><b>${Highcharts.numberFormat(
                  this.percentage,
                  1
                )}%</b></div>
                  <div style='text-align: center; font-size:12px; margin-left: 10px'>R$ ${Highcharts.numberFormat(
                    this.y,
                    0
                  )}</div></div>`;

              return `<div style='text-align: center;'><b>${Highcharts.numberFormat(
                this.percentage,
                1
              )}%</b></div>
                <div style='text-align: center; font-size:12px;'>R$ ${Highcharts.numberFormat(
                  this.y,
                  0
                )}</div>`;
            },
          },
        },
      },
      series: this.valuesFinancialImpact,
    };
  }
}
