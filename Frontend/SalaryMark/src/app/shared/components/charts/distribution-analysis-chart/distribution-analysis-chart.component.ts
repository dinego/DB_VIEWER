import {
  Component,
  OnInit,
  Input,
  OnChanges,
  SimpleChanges,
} from "@angular/core";
import * as Highcharts from "highcharts";
import HC_map from "highcharts/highcharts-more";
import { DistribuitionAnalysisType } from "./distribuition-analysis-type";
// tslint:disable-next-line: max-line-length
import {
  DistribuitionAnalysisChartInput,
  DistribuitionAnalysisCategory,
  DistribuitionAnalysisDrillDown,
} from "./distribuition-analysis-chart-input";
import Drilldown from "highcharts/modules/drilldown";
import defaultFontsHichCharts from "@/shared/view-models/fonts-charts";

Drilldown(Highcharts);
HC_map(Highcharts);

@Component({
  selector: "app-distribution-analysis-chart",
  templateUrl: "./distribution-analysis-chart.component.html",
  styleUrls: ["./distribution-analysis-chart.component.scss"],
})
export class DistributionAnalysisChartComponent implements OnInit, OnChanges {
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
  @Input() showDrillDown: boolean = true;

  @Input() isModal: boolean = false;

  @Input()
  width?: number;

  @Input()
  valuesDistributionAnalysisChart: DistribuitionAnalysisChartInput;

  @Input() typeVisualization: string = "bar";

  // tslint:disable-next-line: only-arrow-functions
  chartCallback: Highcharts.ChartCallbackFunction = function () {}; // optional function, defaults to null

  constructor() {
    const self = this;
    // saving chart reference using chart callback
    this.chartCallback = (chart) => {
      self.chart = chart;
    };
  }

  colorsSeries(key: DistribuitionAnalysisType) {
    const colors = new Map<DistribuitionAnalysisType, string>([
      [DistribuitionAnalysisType.BelowWagePolicy, "#A0527E"],
      [DistribuitionAnalysisType.WithinWagePolicy, "#81AC97"],
      [DistribuitionAnalysisType.AboveWagePolicy, "#5B76A8"],
    ]);

    return colors.get(key);
  }

  ngOnChanges(changes: SimpleChanges) {
    for (const propName in changes) {
      if (changes.hasOwnProperty(propName)) {
        switch (propName) {
          case "typeVisualization": {
            if (
              this.chartOptions &&
              this.chartOptions.chart.type &&
              this.typeVisualization
            ) {
              this.chartOptions.chart.type = this.typeVisualization;
              this.chart.update(this.chartOptions);
              this.updateFlag = true;
            }
            break;
          }
          case "width": {
            if (this.chartOptions && this.chartOptions.width && this.width) {
              this.chartOptions.width = this.width;
              this.chart.update(this.chartOptions);
              this.updateFlag = true;
            }
            break;
          }
          case "valuesDistributionAnalysisChart": {
            if (!changes.valuesDistributionAnalysisChart.firstChange) {
              const result: DistribuitionAnalysisChartInput =
                changes.valuesDistributionAnalysisChart.currentValue;

              if (result && result.drillDown) {
                this.chart.drillUp();
                this.chartOptions.drilldown.series =
                  this.fixDrillDownSeriesChart(result.drillDown);
              }

              this.chartOptions.series =
                result && result.main
                  ? this.fixSeriesChart(result.main, this.showDrillDown)
                  : [];

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

  addCrossIcon(e) {
    const el = document.getElementsByClassName("highcharts-drillup-button");

    // el[0]
    //   .append("text")
    //   .attr("x", 0)
    //   .attr("y", 70)
    //   .attr("font-family", "FontAwesome")
    //   .attr("font-size", function (d) {
    //     return "70px";
    //   })
    //   .text(function (d) {
    //     return "\uf083";
    //   });
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
      lang: {
        drillUpText: "Fechar DrillDown",
      },
      chart: {
        spacingBottom: 0,
        height: componentScope.height,
        width: componentScope.width ? componentScope.width : null,
        type: componentScope.typeVisualization,
        events: {
          drilldown(e) {
            componentScope.addCrossIcon(e);
          },
          drillup() {},
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
                      style: { ...defaultFontsHichCharts.legendLabelsTooltip },
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
            [0, "#839CCC"],
            [1, "#5F7298"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#81AC97"],
            [1, "#668979"],
          ],
        },
        {
          linearGradient: linearGradient,
          stops: [
            [0, "#A0527E"],
            [1, "#713557"],
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
            ...defaultFontsHichCharts.axiesXY,
          },
        },
      },
      yAxis: {
        max: 4,
        gridLineWidth: 0,
        gridLineColor: "#ebebeb",
        tickPositions: [0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100],
        title: {
          text: null,
        },
        labels: {
          enabled: false,
          style: {
            ...defaultFontsHichCharts.axiesXY,
          },
          formatter() {
            return Highcharts.numberFormat(this.value, 0) + "%";
          },
        },
      },
      legend: {
        reversed: true,
        symbolHeight: 18,
        symbolWidth: 18,
        symbolRadius: 2,
        squareSymbol: false,
        layout: "horizontal",
        align: "center",
        verticalAlign: "bottom",
        useHTML: true,
        itemStyle: {
          ...defaultFontsHichCharts.legendLabelsTooltip,
        },
      },
      tooltip: {
        enabled: false,
      },
      plotOptions: {
        series: {
          cursor: componentScope.showDrillDown ? "pointer" : "",
          events: {
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
        bar: {
          stacking: "normal",
          borderWidth: 0,
          maxPointWidth: 80,
          dataLabels: {
            enabled: true,
            style: {
              ...defaultFontsHichCharts.legendLabelsTooltip,
              color: "white",
              textOutline: null,
            },
            formatter() {
              return Highcharts.numberFormat(this.point.y, 0) + "%";
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
              ...defaultFontsHichCharts.legendLabelsTooltip,
              color: "white",
              textOutline: null,
            },
            formatter() {
              return Highcharts.numberFormat(this.point.y, 0) + "%";
            },
          },
        },
        waterfall: {
          borderWidth: 0,
          lineColor: "white",
          dataLabels: {
            enabled: true,
            inside: false,
            style: {
              ...defaultFontsHichCharts.legendLabelsTooltip,
              fontWeight: "bold",
              textOutline: null,
            },
            formatter() {
              return Highcharts.numberFormat(this.point.y, 0) + "%";
            },
          },
        },
      },
      series: componentScope.fixSeriesChart(
        componentScope.valuesDistributionAnalysisChart?.main,
        componentScope?.showDrillDown
      ),
      drilldown: {
        drillUpButton: {
          relativeTo: "spacingBox",
          theme: {
            fill: "white",
            "stroke-width": 1,
            stroke: "#F08E3C",
            r: 15,
            states: {
              hover: {},
            },
          },
          position: {
            align: "left",
            x: 20,
            y: -10,
          },
        },
        activeAxisLabelStyle: {
          ...defaultFontsHichCharts.legendLabelsTooltip,
          fontWeight: "bold",
          textDecoration: "none",
        },
        activeDataLabelStyle: {
          ...defaultFontsHichCharts.legendLabelsTooltip,
          cursor: "cursor",
          color: "white",
          textOutline: null,
          textDecoration: "none",
        },
        series: componentScope.valuesDistributionAnalysisChart?.drillDown
          ? componentScope.fixDrillDownSeriesChart(
              componentScope.valuesDistributionAnalysisChart?.drillDown
            )
          : null,
      },
    };
  }

  fixSeriesChart(
    value: DistribuitionAnalysisCategory[],
    showDrillDown: boolean
  ) {
    if (!value) {
      return;
    }

    const result = [];
    value.forEach((f) => {
      result.push({
        cursor: showDrillDown ? "pointer" : "",
        name: f.name,
        stack: "stack",
        data: f.data.map((m) => ({
          name: m.name,
          y: m.value,
          drilldown: showDrillDown ? m.name : null,
        })),
        point: {
          events: {
            mouseOver: function (e) {
              this.dataLabel.css({
                fontWeight: "bold",
                cursor: showDrillDown ? "pointer" : "",
              });
            },
            mouseOut: function (e) {
              this.dataLabel.css({
                fontWeight: "normal",
                cursor: showDrillDown ? "pointer" : "",
              });
            },
          },
        },
      });
    });

    if (!this.isModal) {
      let arrayForModal: any[] = [];
      result.forEach((element) => {
        if (element.data.length > 5) {
          element.data = element.data.splice(0, 5);
          arrayForModal.push(element);
        } else {
          arrayForModal.push(element);
        }
      });

      return arrayForModal;
    }

    return result;
  }

  fixDrillDownSeriesChart(value: DistribuitionAnalysisDrillDown[]) {
    const result = [];

    value.forEach((f) => {
      result.push({
        name: "",
        color: "white",
        showInLegend: true,
        id: f.itemGrouped,
        type: "waterfall",
        data: f.data.map((m) => ({
          name: m.name,
          y: m.value,
          color: this.colorsSeries(m.type),
        })),
      });
    });
    return result;
  }
}
