import defaultFontsHighcharts from "@/shared/view-models/fonts-charts";
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
import {
  ComparativeAnalyseEnum,
  ComparativeAnalysisChartInput,
} from "./comparative-analysis-chart-input";

@Component({
  selector: "app-comparative-analysis-chart",
  templateUrl: "./comparative-analysis-chart.component.html",
  styleUrls: ["./comparative-analysis-chart.component.scss"],
})
export class ComparativeAnalysisChartComponent implements OnInit, OnChanges {
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

  // input
  @Input() type: number;
  @Input() className = "";
  @Input()
  isWithLine = true;

  @Input()
  height = 500;

  @Input()
  valuesComparativeAnalysis: ComparativeAnalysisChartInput;

  @Input()
  showDataLabels: boolean;

  colorsRangeBasedOnValues = [
    {
      min: 130,
      max: 999,
      color: "#537158",
    },
    {
      min: 121,
      max: 130,
      color: "#668C6C",
    },
    {
      min: 110,
      max: 120.999,
      color: "#81A487",
    },
    {
      min: 103,
      max: 109.999,
      color: "#9FBDA1",
    },
    {
      min: 98,
      max: 102.999,
      color: "#B5CBB7",
    },
    {
      min: 93,
      max: 97.999,
      color: "#D4E1D6",
    },
    {
      min: 89,
      max: 92.999,
      color: "#EBF1EC",
    },
    {
      min: 86,
      max: 88.999,
      color: "#FAEEEC",
    },
    {
      min: 83,
      max: 85.999,
      color: "#F3D9D5",
    },
    {
      min: 78,
      max: 82.999,
      color: "#E2AAA2",
    },
    {
      min: 70,
      max: 77.999,
      color: "#CE7F74",
    },
    {
      min: 0,
      max: 69.999,
      color: "#A4483E",
    },
  ];

  referenceValueMin = 70;
  referenceValueMax = 130;

  labelRef: any = null;
  labelContainerRef: any = null;

  // output
  @Output()
  clickPoint = new EventEmitter<any>();

  // tslint:disable-next-line: only-arrow-functions
  chartCallback: Highcharts.ChartCallbackFunction = function () {}; // optional function, defaults to null

  constructor() {
    const self = this;
    // saving chart reference using chart callback
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

          case "type": {
            if (!changes.type.firstChange) {
              const type = changes.type.currentValue;

              this.chartOptions.chart.type = type === 3 ? "column" : "bar";

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
      2: "white",
      3: "#404040",
    };

    return colors[key];
  }

  click(point: any) {
    this.clickPoint.emit(point.click);
  }

  updateChart() {
    const componentScope = this;

    let colorBack;
    let textColor;

    if (this.valuesComparativeAnalysis.chart.average) {
      const { color } = this.colorsRangeBasedOnValues.find(
        (value) =>
          this.valuesComparativeAnalysis.chart.average >= value.min &&
          this.valuesComparativeAnalysis.chart.average <= value.max
      );
      colorBack = color;
      textColor =
        this.valuesComparativeAnalysis.chart.average <= 102.999 &&
        this.valuesComparativeAnalysis.chart.average >= 83
          ? "#595959"
          : "white";
    }

    this.chartOptions = {
      chart: {
        type: componentScope.type === 3 ? "column" : "bar",
        backgroundColor: "white",
        spacingTop: componentScope.valuesComparativeAnalysis.chart.average
          ? 70
          : 10,
        spacingBottom: 0,
        height: componentScope.height,
        customAverage: 500,
        events: {
          render() {
            const chart = this;
            const chartX = chart.chartWidth;
            const averageValue =
              componentScope &&
              componentScope.valuesComparativeAnalysis &&
              componentScope.valuesComparativeAnalysis.chart &&
              componentScope.valuesComparativeAnalysis.chart.average
                ? componentScope.valuesComparativeAnalysis.chart.average
                : 0;

            const posLabel = averageValue >= 100 ? chartX - 120 : chartX - 110;

            if (componentScope.labelRef) {
              componentScope.labelRef.destroy();
            }

            if (componentScope.labelContainerRef) {
              componentScope.labelContainerRef.destroy();
            }

            componentScope.labelContainerRef = chart.renderer
              .rect(chartX - 125, 5, 80, 40, 4)
              .attr({
                "stroke-width": 2,
                stroke: averageValue > 0 ? colorBack : "transparent",
                fill: averageValue > 0 ? colorBack : "transparent",
                zIndex: 3,
              })
              .add();

            componentScope.labelRef = chart.renderer
              .label(averageValue > 0 ? `${averageValue}%` : "", posLabel, 10)
              .attr({
                zIndex: 4,
              })
              .css({
                fontSize: "22px",
                color: textColor,
                fontWeight: "bold",
                fontFamily: "proxima-nova",
              })
              .add();
          },
          load() {},
        },
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
            ...defaultFontsHighcharts.axiesXY,
          },
        },
      },

      yAxis: {
        gridLineWidth: 0,
        gridLineColor: "#ebebeb",
        tickPositions: [60, 80, 100, 120, 140],
        title: {
          text: null,
        },
        labels: {
          style: {
            ...defaultFontsHighcharts.axiesXY,
          },
          formatter() {
            return Highcharts.numberFormat(this.value, 0) + "%";
          },
        },
      },
      legend: {
        symbolHeight: 10,
        symbolWidth: 50,
        symbolRadius: 2,
        squareSymbol: false,
        layout: "horizontal",
        itemMarginBottom: 7,
        useHTML: true,
        itemStyle: {
          ...defaultFontsHighcharts.legendLabelsTooltip,
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
          color: "#595959",
          fontFamily: "proxima-nova",
          fontWeight: "normal",
          fontSize: "0.7rem",
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
              if (this.type === "column") {
                event.preventDefault();

                if (this.color !== "white") {
                  this.update({ color: "white" });
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
                      value.color === "white" &&
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
                    event.target.chart.chart.series[i].color === "white" &&
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
              ...defaultFontsHighcharts.legendLabelsTooltip,
              textOutline: null,
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
        column: {
          stacking: "normal",
          enableMouseTracking: false,
          maxPointWidth: 30,
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

    const higherValue = Math.max.apply(null, midPointToMaximum);
    const minimunValue = Math.min.apply(null, midPointToMaximum);

    return [
      {
        type: "column",
        name: "white2",
        color: "white",
        data: midPointToMaximum.map((e) => 10),
        stack: "stack",
        maxPointWidth: 60,
        showInLegend: false,
      },

      {
        type: "column",
        name: midpointToMinimumName,
        color: "#CFDDD1",
        data: midPointToMaximum.map((e) => 30),
        stack: "stack",
        maxPointWidth: 60,
      },
      {
        type: "column",
        name: midPointToMaximumName,
        color: "#84A48A",
        data: midPointToMaximum.map((m) => 30),
        stack: "stack",
        maxPointWidth: 60,
      },
      {
        type: "column",
        name: "white1",
        color: "white",
        data: midPointToMaximum.map((m) => 70),
        stack: "stack",
        maxPointWidth: 60,
        showInLegend: false,
      },

      {
        type: "line",
        name: peopleFrontMidPointName,
        color: "black",
        data: peopleFrontMidPoint.map((m) => {
          if (m.percentage < this.minValue) {
            return { y: this.minValue, fy: m.percentage, click: m.click };
          }
          if (m.percentage > this.maxValue) {
            return { y: this.maxValue, fy: m.percentage, click: m.click };
          }

          return { y: m.percentage, fy: m.percentage, click: m.click };
        }),
      },
    ];
  }
}
