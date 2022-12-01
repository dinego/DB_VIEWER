import {
  Component,
  OnInit,
  Input,
  EventEmitter,
  Output,
  ViewChild,
  ElementRef,
  Renderer2,
  Inject,
} from "@angular/core";

import commonLocales from "@/locales/common";
import { ReportTypeEnum } from "@/shared/enum/report-type-enum";
import { DOCUMENT } from "@angular/common";
import { ReportService } from "@/shared/services/reports/report.service";
import { NgxSpinnerService } from "ngx-spinner";
import { IDownloadMyReport } from "@/shared/interfaces/my-reports";

@Component({
  selector: "app-report-item",
  templateUrl: "./report-item.component.html",
  styleUrls: ["./report-item.component.scss"],
})
export class ReportItemComponent implements OnInit {
  @Input() id: string;
  @Input() title: string;
  @Input() date: string;
  @Input() image: any;
  @Output() download = new EventEmitter<IDownloadMyReport>();
  @Input() reportType: ReportTypeEnum;
  @Input() embedHtml: string;
  @Input() embedScript: string;
  @Input() fileName: string;

  @ViewChild("HtmlEmbeded", { static: true }) private HtmlEmbeded: ElementRef;

  reportTypeEnum = ReportTypeEnum;
  isCollapse = true;

  constructor(
    private renderer2: Renderer2,
    @Inject(DOCUMENT) private document,
    private myReportService: ReportService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}

  public commonLocales = commonLocales;

  ngOnInit(): void {
    const options: Intl.DateTimeFormatOptions = {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
    };
    this.date = new Date(this.date).toLocaleDateString("pt", options);
    if (this.reportType === ReportTypeEnum.Embed) this.embedReport();
  }

  onClickEvent() {
    this.download.emit({ id: this.id, fileName: this.fileName });
  }

  async viewMore() {
    await this.myReportService.registerMyReportLog(+this.id).toPromise();
    this.ngxSpinnerService.hide();
    this.isCollapse = !this.isCollapse;
  }

  embedReport() {
    this.HtmlEmbeded.nativeElement.insertAdjacentHTML(
      "beforeend",
      this.embedHtml
    );
    const script = this.renderer2.createElement("script");
    script.text = this.embedScript;
    this.renderer2.appendChild(this.document.body, script);
  }
}
