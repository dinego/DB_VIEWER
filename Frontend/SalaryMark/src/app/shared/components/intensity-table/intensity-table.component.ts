import {
  FrameworkColumnsMainEnum,
  FrameworkTableColsEnum,
} from "@/shared/enum/framework-columns-main-enum";
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
  Renderer2,
  ViewChild,
} from "@angular/core";
import { MediaObserver } from "@angular/flex-layout";

@Component({
  selector: "app-intensity-table",
  templateUrl: "./intensity-table.component.html",
  styleUrls: ["./intensity-table.component.scss"],
})
export class IntensityTableComponent implements OnInit {
  @Input() data: any[];
  @Input() minBar: number;
  @Input() maxBar: number;
  @Input() displayInModal: boolean;
  @Input() query: string;

  @ViewChild("basePercent") basePercent: ElementRef;
  @ViewChild("tableIntensity") tableIntensity: ElementRef;
  @ViewChild("scrollTable") scrollTable: ElementRef;
  @ViewChild("bodyTable") bodyTable: ElementRef;
  @ViewChild("headTable") headTable: ElementRef;

  @Output() scrollGetFramework = new EventEmitter<boolean>();

  public frameworkTableColsEnum = FrameworkTableColsEnum;
  public dataRefactory: any[] = [];
  public tableClass = "";
  public dashedLine: any;
  public copyData: any[] = [];
  public baseMid: number;

  constructor(
    elRef: ElementRef,
    private renderer: Renderer2,
    private mediaObserver: MediaObserver,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    await this.copyDataInit();
    await this.configureScreen();
    await this.setDataTable();
  }

  setBaseMid() {
    this.baseMid = Math.floor(this.maxBar / 100) * 100;
  }

  copyDataInit() {
    this.copyData.push(...this.data);
  }

  setDataTable() {
    let index = 0;
    this.copyData.forEach((items) => {
      let tempItems = [];
      items.forEach((item) => {
        if (
          item.colPos === FrameworkTableColsEnum.Unit ||
          item.colPos === FrameworkTableColsEnum.Salary ||
          item.colPos === FrameworkTableColsEnum.Position ||
          item.colPos === FrameworkTableColsEnum.Percent
        ) {
          const dataRef = {
            colPos: item.colPos,
            value: item.value,
          };
          if (item.colPos === FrameworkTableColsEnum.Unit) {
            tempItems.splice(0, 0, dataRef);
          } else {
            tempItems.push(dataRef);
          }
        }
      });

      tempItems.splice(2, 0, {
        colPos: FrameworkTableColsEnum.PositionId,
        value: items[index].salaryBaseId,
      });
      this.dataRefactory.push(tempItems);
    });
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg";
          break;
        case "md":
          this.tableClass = "ngx-custom-md";
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });
  }

  public onScrollDown() {
    this.scrollGetFramework.emit(true);
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }

  onScroll(event) {
    if (!this.displayInModal) {
      const baseToScrollLine =
        this.basePercent.nativeElement.getBoundingClientRect();

      this.renderer.setStyle(
        this.dashedLine,
        "top",
        `${baseToScrollLine.y - 276}px`
      );
    }
  }

  ngAfterViewInit() {
    this.createDashedLine();
  }

  ngOnChanges() {
    this.copyData.push(...this.data);
    this.setDataTable();
    this.setBaseMid();
  }

  createDashedLine(isResize?: boolean) {
    const tableIntensity = this.tableIntensity.nativeElement;
    let tableWidthHalf = this.tableIntensity.nativeElement.offsetWidth / 2;

    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg";
          break;
        case "md":
          this.tableClass = "ngx-custom-md";
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });

    const sizeScreen =
      this.tableClass == "ngx-custom-lg" || this.tableClass == "ngx-custom";

    if ((!this.displayInModal && isResize) || sizeScreen)
      tableWidthHalf += this.tableIntensity.nativeElement.offsetWidth * 0.267;
    else if (!isResize && this.displayInModal)
      tableWidthHalf = window.innerWidth / 2 + window.innerWidth * 0.1;

    const dashedDiv = document.createElement("div");
    dashedDiv.className = "dashed";
    dashedDiv.id = "dashedMidPoint";
    dashedDiv.innerHTML = "";
    this.dashedLine = dashedDiv;

    const headTableHeight = this.headTable.nativeElement.offsetHeight;
    const bodyTableHeight = this.bodyTable.nativeElement.offsetHeight;

    const elTable = this.scrollTable.nativeElement;

    const el = dashedDiv;
    this.renderer.setStyle(el, "color", "white");
    this.renderer.setStyle(el, "border", "2px dashed black");
    this.renderer.setStyle(el, "position", "absolute");
    if (!this.displayInModal && isResize)
      this.renderer.setStyle(el, "top", `${headTableHeight}px`);
    else this.renderer.setStyle(el, "top", `${headTableHeight + 200}px`);
    this.renderer.setStyle(
      el,
      "left",
      `${!isResize ? tableWidthHalf - 58 : tableWidthHalf - 67}px`
    );
    this.renderer.setStyle(el, "z-index", "1");
    this.renderer.setStyle(el, "margin-left", "10px");
    this.renderer.setStyle(
      el,
      "height",
      !this.displayInModal
        ? `${bodyTableHeight}px`
        : `${elTable.offsetHeight - 50}px`
    );
    this.renderer.setStyle(el, "width", `1px`);

    this.renderer.setStyle(
      elTable,
      "height",
      `${bodyTableHeight}px !important`
    );

    const container = elTable;
    container.appendChild(dashedDiv);
  }

  @HostListener("window:resize", ["$event"])
  onResize(event) {
    document.getElementById("dashedMidPoint").remove();
    this.createDashedLine(true);
  }
}
