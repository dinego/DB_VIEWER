import defaultFonts from "@/shared/view-models/fonts-charts";
import {
  Component,
  OnInit,
  OnChanges,
  SimpleChanges,
  Input,
} from "@angular/core";
import * as Highcharts from "highcharts";
import { PositionsDashboardChartInput } from "./positions-dashboard-chart-input";

@Component({
  selector: "app-positions-dashboard-chart",
  templateUrl: "./positions-dashboard-chart.component.html",
  styleUrls: ["./positions-dashboard-chart.component.scss"],
})
export class PositionsDashboardChartComponent implements OnInit, OnChanges {
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
  // input
  @Input()
  valuesPositionsDashboard: PositionsDashboardChartInput;

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
          case "valuesPositionsDashboard": {
            if (!changes.valuesPositionsDashboard.firstChange) {
              this.chartOptions.series = this.fixDataChart(
                changes.valuesPositionsDashboard.currentValue
              );

              this.chartOptions.title.text = `<div style='text-align: center;font-size: 1.6rem;'>
                                                <b>${changes.valuesPositionsDashboard.currentValue.amountPositions}</b>
                                                <div style='text-align: center; font-weight: 300'>TOTAL</div>
                                              </div>`;

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

  updateChart() {
    const componentScope = this;

    this.chartOptions = {
      chart: {
        height: componentScope.height,
        type: "pie",
      },
      credits: {
        enabled: false,
      },
      exporting: { enabled: false },
      tooltip: {
        enabled: true,
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
          const value = this.point.name ? this.point.name.toLowerCase() : "";
          if (this.series.index === 0) {
            return false;
          } else {
            return `<div style='width: 100px'><b>${
              this.point.positions
            }</b> cargos <br/><b>${Highcharts.numberFormat(
              this.point.occupantsPercentage,
              0
            )}%</b> ${value}</div>`;
          }
        },
      },
      legend: {
        symbolHeight: 16,
        symbolWidth: 16,
        symbolRadius: 2,
        squareSymbol: false,
        itemMarginBottom: 7,
        layout: "horizontal",
        useHTML: true,
        itemStyle: {
          ...defaultFonts.legendLabelsTooltip,
        },
        x: 15,
      },
      title: {
        text: `<div style='text-align: center; font-size: 22px;'>
                <b>${componentScope.valuesPositionsDashboard.amountPositions}</b>
              </div>
              <div style='text-align: center; font-weight: 300'>TOTAL</div>`,
        style: {
          ...defaultFonts.title,
        },
        useHTML: true,
        align: "center",
        verticalAlign: "middle",
        y: -5,
      },
      yAxis: {},
      plotOptions: {
        series: {
          point: {
            events: {
              legendItemClick() {
                return false;
              },
            },
          },
          marker: {
            enabled: false,
          },
        },
        pie: {
          shadow: false,
          borderWidth: 0,
          dataLabels: {
            enabled: true,
            softConnector: false,
            distance: 15,
            formatter() {
              return this.y + "%";
            },
            style: {
              ...defaultFonts.legendLabelsTooltip,
              textOutline: null,
              fontWeight: "700",
            },
          },
        },
      },
      series: componentScope.fixDataChart(
        componentScope.valuesPositionsDashboard
      ),
    };
  }

  fixDataChart(value: PositionsDashboardChartInput) {
    return [
      {
        data: [
          {
            color: "#7F7F7F",
            name: value.name,
            y: value.percentage,
          },
          {
            color: "#E7EDE8",
            isAux: true,
            name: "",
            y: 100 - value.percentage,
            dataLabels: {
              enabled: false,
            },
          },
        ],
        size: "80%",
        innerSize: "65%",
        showInLegend: true,
      },
      {
        data: [
          {
            color: "white",
            name: value.name,
            y: value.percentage,
            positions: value.amountPositions,
            occupantsPercentage: value.occupantsPercentage,
          },
        ],
        size: "50%",
        innerSize: "0%",
        dataLabels: {
          enabled: false,
        },
        showInLegend: false,
      },
    ];
  }
}
