import {
  Component,
  OnInit,
  SimpleChanges,
  Input,
  Output,
  EventEmitter,
} from "@angular/core";
import * as Highcharts from "highcharts";
import {
  ComparativeAnalyseEnum,
  ComparativeAnalysisChartInput,
} from "../comparative-analysis-chart/comparative-analysis-chart-input";
import colorsRangeBasedOnValues from "./colorsRange";
import defaultFonts from "@/shared/view-models/fonts-charts";

@Component({
  selector: "app-comparative-analysis-chart-dashboard",
  templateUrl: "./comparative-analysis-chart-dashboard.component.html",
  styleUrls: ["./comparative-analysis-chart-dashboard.component.scss"],
})
export class ComparativeAnalysisChartDashboardComponent implements OnInit {
  maxValue = 140;
  minValue = 60;

  chart: Highcharts.Chart;

  Highcharts: typeof Highcharts = Highcharts; // required
  chartConstructor = "chart"; // optional string, defaults to 'chart'
  chartOptions: any;

  updateFlag = false; // optional boolean
  oneToOneFlag = true; // optional boolean, defaults to false
  runOutsideAngular = true; // optional boolean, defaults to false

  chartInstance: Highcharts.Chart;

  @Input() className = "";
  @Input() isWithLine = true;
  @Input() isModal = false;
  @Input() height = 500;
  @Input() width?: number;
  @Input() valuesComparativeAnalysis: ComparativeAnalysisChartInput;

  @Input() showDataLabels: boolean;

  colorsRangeBasedOnValues = colorsRangeBasedOnValues;

  referenceValueMin = 70;
  referenceValueMax = 130;

  labelRef: any = null;
  labelContainerRef: any = null;

  // output
  @Output() clickPoint = new EventEmitter<any>();

  // tslint:disable-next-line: only-arrow-functions
  chartCallback: Highcharts.ChartCallbackFunction = function () {}; // optional function, defaults to null

  constructor() {
    const self = this;
    this.chartCallback = (chart) => {
      self.chart = chart;
    };
  }

  ngOnChanges(changes: SimpleChanges) {
    for (const propName in changes) {
      if (changes.hasOwnProperty(propName)) {
        switch (propName) {
          case "valuesComparativeAnalysis": {
            if (!changes.valuesComparativeAnalysis.firstChange) {
              this.chartOptions.series = this.fixDataChart(
                changes.valuesComparativeAnalysis.currentValue
              );

              (this.chartOptions.xAxis.categories = this.getCategories(
                changes.valuesComparativeAnalysis.currentValue
              )),
                this.chart.update(this.chartOptions);

              this.updateFlag = true;
            }
            break;
          }
          case "isWithLine": {
            if (!changes.isWithLine.firstChange) {
              const isWithLine = changes.isWithLine.currentValue;

              this.chartOptions.plotOptions.line.lineWidth = isWithLine ? 1 : 0;
              this.chartOptions.plotOptions.line.states.hover.lineWidthPlus =
                isWithLine ? 2 : 0;

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

  colorsSeries(key: number) {
    const colors = {
      0: "#84A48A",
      1: "#CFDDD1",
      2: "#FFFFFF",
      3: "#404040",
    };

    return colors[key];
  }

  click(point: any) {
    this.clickPoint.emit(point.click);
  }

  updateChart() {
    const componentScope = this;

    this.chartOptions = {
      chart: {
        backgroundColor: "#FFFFFF",
        spacingTop: 10,
        spacingBottom: 0,
        height: componentScope.height,
        width: componentScope.width ? componentScope.width : null,
        customAverage: 500,
      },
      credits: {
        enabled: false,
      },
      exporting: { enabled: false },
      title: {
        text: null,
      },
      xAxis: {
        lineColor: "white",
        categories: componentScope.getCategories(
          componentScope.valuesComparativeAnalysis
        ),
        labels: {
          style: {
            ...defaultFonts.axiesXY,
          },
        },
      },

      yAxis: {
        gridLineWidth: 0,
        gridLineColor: "#ebebeb",
        tickPositions: [70, 80, 90, 100, 110, 120, 130],
        title: {
          text: null,
        },
        labels: {
          style: {
            ...defaultFonts.axiesXY,
          },
          formatter() {
            return Highcharts.numberFormat(this.value, 0) + "%";
          },
        },
      },
      legend: {
        symbolHeight: 18,
        symbolWidth: 18,
        symbolRadius: 2,
        squareSymbol: false,
        layout: "horizontal",
        align: "left",
        verticalAlign: "top",
        itemMarginBottom: 7,
        useHTML: true,
        itemStyle: {
          ...defaultFonts.legendLabelsTooltip,
        },
      },
      tooltip: {
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
          return (
            "<div style='text-align: center;'><b>" +
            this.key +
            "</b></div>" +
            "<div style='text-align: center;'>" +
            Highcharts.numberFormat(this.point.fy, 0) +
            "%</div>"
          );
        },
      },
      plotOptions: {
        series: {
          cursor: componentScope.showDataLabels ? "pointer" : "",
          events: {
            click(event) {
              componentScope.click(event.point.options);
            },
            legendItemClick(event) {
              if (this.type === "bar") {
                event.preventDefault();

                if (this.color !== "white") {
                  this.update({ color: "#FFFFFF" });
                  this.legendSymbol.attr({
                    fill: "lightgray",
                  });

                  // tslint:disable-next-line: prefer-for-of
                  for (
                    let i = 0;
                    i < event.target.chart.chart.series.length;
                    i++
                  ) {
                    const value = event.target.chart.chart.series[i];

                    if (
                      value.color === "red" &&
                      event.target.chart.chart.series[i].legendSymbol
                    ) {
                      event.target.chart.chart.series[i].legendSymbol.attr({
                        fill: "lightgray",
                      });
                    }
                  }

                  return true;
                }

                this.update({ color: componentScope.colorsSeries(this.index) });

                this.legendSymbol.attr({
                  fill: componentScope.colorsSeries(this.index),
                });

                // tslint:disable-next-line: prefer-for-of
                for (
                  let i = 0;
                  i < event.target.chart.chart.series.length;
                  i++
                ) {
                  if (
                    event.target.chart.chart.series[i].color === "red" &&
                    event.target.chart.chart.series[i].legendSymbol
                  ) {
                    event.target.chart.chart.series[i].legendSymbol.attr({
                      fill: "lightgray",
                    });
                  }
                }

                return true;
              }
            },
          },
        },
        line: {
          animation: {
            duration: 2000,
          },
          color: "black",
          lineWidth: componentScope.isWithLine ? 1 : 0,
          marker: {
            lineWidth: 3,
            lineColor: "black",
            fillColor: "black",
            states: {
              inactive: {
                opacity: 1,
              },
              hover: {
                radiusPlus: 5,
                lineWidthPlus: 2,
              },
            },
          },
          states: {
            hover: {
              lineWidthPlus: componentScope.isWithLine ? 2 : 0,
            },
          },
          dataLabels: {
            useHTML: true,
            outside: true,
            enabled: true,
            color: "black",
            verticalAlign: "top",
            padding: 10,
            style: {
              ...defaultFonts.legendLabelsTooltip,
            },
            formatter() {
              const imagePath = "./assets/imgs/svg/arrow_down_chart.svg";
              switch (this.point.y) {
                case componentScope.minValue:
                  return componentScope.showDataLabels
                    ? `${this.point.y}% <img style="margin-top:15px;margin-right:30px;" src="${imagePath}"/>`
                    : `<img style="margin-top:15px;margin-right:30px;" src="${imagePath}"/>`;
                case componentScope.maxValue:
                  return componentScope.showDataLabels
                    ? `${this.point.y}% <img style="margin-top:-25px;margin-right:30px;transform:rotate(180deg);" src="${imagePath}"/>`
                    : `<img style="margin-top:-25px;margin-right:30px;transform:rotate(180deg);" src="${imagePath}"/>`;
                default:
                  return componentScope.showDataLabels
                    ? `${this.point.y}%`
                    : "";
              }
            },
          },
        },
        bar: {
          stacking: "normal",
          enableMouseTracking: false,
          maxPointWidth: 80,
          borderWidth: 0,

          dataLabels: {
            enabled: false,
          },

          states: {
            inactive: {
              opacity: 1,
            },
            hover: {
              halo: false,
              enabled: false,
            },
          },
        },
      },
      series: componentScope.fixDataChart(
        componentScope.valuesComparativeAnalysis
      ),
    };
  }

  getCategories(value: ComparativeAnalysisChartInput) {
    return value && value.chart && value.chart.chart
      ? value.chart.chart[0].data.map((m) => m.name)
      : "";
  }

  fixDataChart(value: ComparativeAnalysisChartInput) {
    let midPointToMaximumName = "";
    let midPointToMaximum = [];

    let midpointToMinimumName = "";
    let midpointToMinimum = [];

    let peopleFrontMidPointName = "";
    let peopleFrontMidPoint = [];

    if (value && value.chart) {
      value.chart.chart.forEach((element) => {
        switch (element.type) {
          case ComparativeAnalyseEnum.MidPointToMaximum:
            let media = element.data.map((m) => m.percentage);
            midPointToMaximum = element.data.map((m) => m.percentage);
            midPointToMaximumName = element.name;
            break;

          case ComparativeAnalyseEnum.MidpointToMinimum:
            midpointToMinimum = element.data.map((m) => m.percentage);
            midpointToMinimumName = element.name;
            break;

          case ComparativeAnalyseEnum.PeopleFrontMidPoint:
            peopleFrontMidPoint = element.data.map((m) => ({
              percentage: m.percentage,
              click: m.click,
            }));
            peopleFrontMidPointName = element.name;
            break;

          default:
            break;
        }
      });
    }
    if (!this.isModal) {
      midPointToMaximum = midPointToMaximum.splice(0, 5);
      midpointToMinimum = midpointToMinimum.splice(0, 5);
      peopleFrontMidPoint = peopleFrontMidPoint.splice(0, 5);
    }
    return [
      {
        showInLegend: false,
        type: "bar",
        name: "empty02",
        color: "#DEDEDE",
        data: midPointToMaximum,
        stack: "stack",
      },
      {
        type: "bar",
        name: midPointToMaximumName,
        color: "#84A48A",
        data: midPointToMaximum.map((m) => m - 100),
        stack: "stack",
      },
      {
        type: "bar",
        name: midpointToMinimumName,
        color: "#CFDDD1",
        data: midpointToMinimum.map((e) => 100 - e),
        stack: "stack",
      },
      {
        showInLegend: false,
        type: "bar",
        name: "empty01",
        color: "#DEDEDE",
        data: midpointToMinimum,
        stack: "stack",
      },
      {
        type: "line",
        name: peopleFrontMidPointName,
        color: "black",
        data: peopleFrontMidPoint.map((m) => {
          if (m.percentage < this.minValue) {
            return { x: this.minValue, fy: m.percentage, click: m.click };
          }
          if (m.percentage > this.maxValue) {
            return { x: this.maxValue, fy: m.percentage, click: m.click };
          }

          return { y: m.percentage, fy: m.percentage, click: m.click };
        }),
      },
    ];
  }
}
