import { DOCUMENT } from "@angular/common";
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  ElementRef,
  ViewChild,
  Renderer2,
  Inject,
  Output,
  EventEmitter,
} from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { Publications } from "@/shared/services/studies/common/publications";
import { UserService } from "@/shared/services/user/user.service";
import { UserParameterAccess } from "@/shared/models/user-parameter";
import { CommonService } from "@/shared/services/common/common.service";
import { NgxSpinnerService } from "ngx-spinner";
import { Modules } from "@/shared/models/modules";

@Component({
  selector: "app-publication-detail",
  templateUrl: "./publication-detail.component.html",
  styleUrls: ["./publication-detail.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PublicationDetailComponent implements OnInit {
  publication: Publications;
  isSharedView = false;

  @ViewChild("embedHtml", { static: true }) private embedHtml: ElementRef;

  userParameterAccess: UserParameterAccess = new UserParameterAccess();

  modalRef: BsModalRef;

  @Output() publicationEmitter = new EventEmitter<number>();

  constructor(
    public bsModalRef: BsModalRef,
    private renderer2: Renderer2,
    private bsModalService: BsModalService,
    private userService: UserService,
    private commonService: CommonService,
    private NgxSpinnerService: NgxSpinnerService,
    @Inject(DOCUMENT) private document
  ) {}

  ngOnInit(): void {
    this.loadPermissions();

    if (this.publication.html && this.publication.script) {
      this.embedHtml.nativeElement.insertAdjacentHTML(
        "beforeend",
        this.publication.html
      );

      const script = this.renderer2.createElement("script");
      script.text = this.publication.script;
      this.renderer2.appendChild(this.document.body, script);
    } else if (this.publication.iFrame) {
      this.embedHtml.nativeElement.insertAdjacentHTML(
        "beforeend",
        this.publication.iFrame
          .replace("<div ", '<div style="width:100%" ')
          .replace("<iframe ", '<iframe style="width:100%" ')
      );
    }
  }

  private async loadPermissions() {
    this.userParameterAccess.showUsers = await this.userService.canAccessUser();
    this.userParameterAccess.showPjSettings =
      await this.commonService.hidePjSettings();
    this.userParameterAccess.showHourlyBasis =
      await this.commonService.canAccesHourlyBasis();
    this.NgxSpinnerService.hide();
  }

  share() {
    this.publicationEmitter.emit(this.publication.id);
  }
}
