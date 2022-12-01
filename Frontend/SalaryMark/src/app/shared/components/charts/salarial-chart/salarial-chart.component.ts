import { ComponentFactoryClass } from "@/shared/common/component-factory";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { IPermissions } from "@/shared/models/token";
import defaultFontsHighcharts from "@/shared/view-models/fonts-charts";
import defaultFonts from "@/shared/view-models/fonts-charts";
import {
  Component,
  OnInit,
  SimpleChanges,
  Input,
  Output,
  EventEmitter,
  Injector,
  Compiler,
} from "@angular/core";
import * as Highcharts from "highcharts";
import { SalaryChartInput } from "./salarial-chart-input";
import { TooltipComponent } from "./tooltip/tooltip.component";
import { TooltipModule } from "./tooltip/tooltip.module";

@Component({
  selector: "app-salarial-chart",
  templateUrl: "./salarial-chart.component.html",
  styleUrls: ["./salarial-chart.component.scss"],
})
export class SalarialChartComponent implements OnInit {
  chart: Highcharts.Chart;

  Highcharts: typeof Highcharts = Highcharts; // required
  chartConstructor = "chart"; // optional string, defaults to 'chart'
  chartOptions: any;

  updateFlag = false; // optional boolean
  oneToOneFlag = true; // optional boolean, defaults to false
  runOutsideAngular = true; // optional boolean, defaults to false

  chartInstance: Highcharts.Chart;

  referenceValueMin = 70;
  referenceValueMax = 130;

  labelRef: any = null;
  labelContainerRef: any = null;

  @Input()
  showDataLabels: boolean;

  @Input()
  isExport: boolean;

  @Input()
  height: number;

  @Input()
  valuesSalaryChartInput: SalaryChartInput[];
  @Input()
  showOccupantCLT: boolean;
  @Input()
  showOccupantPJ: boolean;
  @Input()
  hoursType: HourlyBasisEnum;

  @Output()
  clickPoint = new EventEmitter<any>();

  @Input()
  maxValue: number;

  @Input() permissions: IPermissions;

  @Input() gsmGlobalLabel: string;

  chartCallback: Highcharts.ChartCallbackFunction = function () {}; // optional function, defaults to null
  constructor(private injector: Injector, private compiler: Compiler) {
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
          case "valuesSalaryChartInput": {
            if (!changes.valuesSalaryChartInput.firstChange) {
              const result: SalaryChartInput[] =
                changes.valuesSalaryChartInput.currentValue;

              this.chartOptions.chart.scrollablePlotArea =
                this.setScrollPlotArea(this.isExport);

              this.chartOptions.series = this.fixSeriesChart(result);
              this.chartOptions.chart.height = this.height;

              this.chartOptions.yAxis.max =
                this.maxValue + this.maxValue * 0.15;

              this.chartOptions.yAxis.min = 1000;

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

  setScrollPlotArea(isExport: boolean) {
    if (
      !this.valuesSalaryChartInput.some((s) => s.data.length > 10) &&
      isExport
    )
      return undefined;

    const size = this.valuesSalaryChartInput[0].data.length;
    const minWidth = size * 40;
    return { minWidth: minWidth, scrollPositionX: 1, opacity: 0 };
  }

  getMinimumWidth() {
    const size = this.valuesSalaryChartInput[0].data.length;
    const minWidth = size * 40;
    return minWidth < 850 ? 850 : minWidth;
  }

  updateChart() {
    const componentScope = this;
    const component = new ComponentFactoryClass<
      TooltipModule,
      TooltipComponent
    >(this.injector, this.compiler).createComponent(
      TooltipModule,
      TooltipComponent
    );

    this.chartOptions = {
      chart: {
        spacingTop: 20,
        spacingBottom: 0,
        type: "column",
        height: componentScope.height,
        width: componentScope.isExport
          ? componentScope.getMinimumWidth()
          : undefined,
        scrollablePlotArea: componentScope.setScrollPlotArea(
          componentScope.isExport
        ),
      },
      credits: {
        enabled: false,
      },
      exporting: { enabled: false },
      title: {
        text: null,
      },
      tooltip: {
        snap: 0,
        useHTML: true,
        hideDelay: 2000,
        //outside: true,
        style: {
          pointerEvents: "all",
          //cursor: "pointer",
        },
        // stickOnContact: true,
        formatter: function () {
          var index = this.point.index;
          var data = componentScope.valuesSalaryChartInput;

          const midpoint =
            componentScope.hoursType == HourlyBasisEnum.HourSalary
              ? componentScope.Highcharts.numberFormat(this.y, 3, ",")
              : componentScope.Highcharts.numberFormat(this.y, 0, ",", ".");

          const gsm =
            data[0] &&
            data[0].data &&
            data[0].data[index] &&
            data[0].data[index].gsm
              ? data[0].data[index].gsm.toString()
              : "0";

          component.instance.permissions = componentScope.permissions;
          component.instance.positions = data
            ? data[2].data[index].positions
            : null;
          component.instance.midPoint = midpoint;
          component.instance.gsmGlobalLabel = componentScope.gsmGlobalLabel;
          component.instance.gsm = gsm;
          component.instance.maxValue = componentScope.maxValue;
          component.instance.showOccupantCLT = componentScope.showOccupantCLT;
          component.instance.showOccupantPJ = componentScope.showOccupantPJ;
          component.changeDetectorRef.detectChanges();
          // Return the tooltip html to highcharts
          return component.location.nativeElement.outerHTML;
        },
      },
      xAxis: {
        lineColor: "gray",
        type: "category",
        labels: {
          style: {
            ...defaultFonts.axiesXY,
          },
        },
      },
      yAxis: {
        minPadding: 20,
        lineColor: "gray",
        labels: {
          style: {
            ...defaultFontsHighcharts.axiesXY,
          },
          formatter() {
            return Highcharts.numberFormat(this.value, 0);
          },
        },
        min: 1000,
        max: componentScope.maxValue + componentScope.maxValue * 0.15,
        gridLineWidth: 1,
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
      plotOptions: {
        line: {
          animation: {
            duration: componentScope.isExport ? 0 : 1000,
          },
          color: "gray",
          lineWidth: 1,
          marker: {
            lineWidth: 2,
            lineColor: "transparent",
          },
          states: {
            lineWidth: 1,
            hover: {
              lineWidth: 1,
              lineWidthPlus: 1,
            },
          },
          dataLabels: {
            useHTML: true,
            outside: true,
            enabled: true,
            color: "#404040",
            verticalAlign: "top",
            style: {
              ...defaultFontsHighcharts.axiesXY,
              fontWeight: "bold",
              fontSize: 10,
              textOutline: null,
            },
            formatter: function () {
              return componentScope.hoursType == HourlyBasisEnum.HourSalary
                ? componentScope.Highcharts.numberFormat(
                    getValueByDataName(
                      this.point.name,
                      componentScope.valuesSalaryChartInput
                    ),
                    2,
                    ","
                  )
                : componentScope.Highcharts.numberFormat(
                    getValueByDataName(
                      this.point.name,
                      componentScope.valuesSalaryChartInput
                    ),
                    0,
                    ",",
                    "."
                  );
            },
          },
        },
        column: {
          animation: {
            duration: componentScope.isExport ? 0 : 1000,
          },
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
      series: componentScope.fixSeriesChart(
        componentScope.valuesSalaryChartInput
      ),
    };
  }
  fixSeriesChart(value: SalaryChartInput[]) {
    const result = [];

    value.forEach((f, index) => {
      result.push({
        showInLegend: false,
        type: "column",
        name: f.name,
        color: f.type === 1 ? "#EBEBEB" : f.type === 3 ? "#CFDDD1" : "#7FA787",
        data: f.data.map((m) => ({
          name: m.name,
          y:
            f.name === "data1"
              ? this.maxValue * 1000
              : index === 3
              ? m.value - m.value * 0.35
              : f.name === "data4"
              ? m.value / 5
              : m.value / 4,
        })),
        stack: "stack",
      });
    });

    const midPointLine: any[] = [];
    value.forEach((f) => {
      if (f.type === 3) midPointLine.push(f);
    });

    midPointLine.forEach((f) => {
      result.push({
        showInLegend: false,
        name: f.name,
        stack: "stack",
        type: "line",
        marker: {
          lineWidth: 3,
          fillColor: "#333333",
        },
        color: "#333333",
        data: f.data.map((m) => ({
          name: m.name,
          y: parseInt((m.value - m.value * 0.102).toFixed(3).toString()),
        })),
      });
    });

    return result;
  }
}

function getValueByDataName(
  name: string,
  valuesSalaryChartInput: SalaryChartInput[]
): number {
  return valuesSalaryChartInput[1].data.find((f) => f.name === name).value;
}
