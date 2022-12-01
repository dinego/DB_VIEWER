import locales from "@/locales/common";
import { Clipboard } from "@angular/cdk/clipboard";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { StudiesService } from "@/shared/services/studies/studies.service";
import {
  OrderPublication,
  Publications,
  StudyShared,
} from "@/shared/services/studies/common/publications";
import orders from "./common/order-publications";
import { applyLink } from "@/shared/common/functions";
import { BlockedContentComponent } from "@/shared/components/blocked-content/blocked-content.component";
import { PublicationDetailComponent } from "@/shared/components/publication-detail/publication-detail.component";
import { OrderTypeEnum } from "@/shared/enum/order-type-enum";
import { StudyTypeENUM } from "./common/studyTypeEnum";
import { Modules } from "@/shared/models/modules";
import { CommonService } from "@/shared/services/common/common.service";
import { environment } from "src/environments/environment";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";
import { ActivatedRoute } from "@angular/router";
import { ScrollService } from "@/shared/services/scroll/scroll.service";
import { DataShare } from "./common/share-data";

@Component({
  selector: "app-studies-publications",
  templateUrl: "./studies-publications.component.html",
  styleUrls: ["./studies-publications.component.scss"],
})
export class StudiesPublicationsComponent implements OnInit {
  @Output() reflow = new EventEmitter<boolean>();
  @Input() input: IDialogInput;

  public locales = locales;
  public showModal = false;
  public isShare: boolean;
  public modalRef?: BsModalRef;
  public publications: Publications[] = [];
  public publicationsCopy: Publications[] = [];
  public pageSize: number = 20;
  public orders: OrderPublication[] = orders;
  public checkboxesId: string;
  public resultTerm: string;
  public myPage = 1;
  public isSharedView = false;
  public orderType: number = 1;
  public orderTypes = this.convertEnumToArray();
  public withPermission: boolean;
  public query: string;
  public modules = Modules;
  public inputShareModal: IDialogInput;
  public shareURL: string;
  public email: string;
  public permissions: IPermissions;
  public publicationShare: Publications;
  public secretKey: string;
  public studyShared: StudyShared = null;

  constructor(
    private studiesService: StudiesService,
    private bsModalService: BsModalService,
    private ngxSpinnerService: NgxSpinnerService,
    private commonService: CommonService,
    private clipboard: Clipboard,
    private tokenService: TokenService,
    private route: ActivatedRoute
  ) {}

  async ngOnInit() {
    await this.tryGetSecretKey();
    await this.getAllStudies();
    await this.setInputShare();
    await this.getPermissions();
  }

  onScrollDown() {
    this.myPage++;
    this.getAllStudies(true);
  }

  async tryGetSecretKey() {
    this.secretKey = this.route.snapshot.paramMap.get("key");
  }

  async setInputShare() {
    this.inputShareModal = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };
  }

  async getPermissions() {
    this.permissions = this.tokenService.getPermissions();
  }

  async getAllStudies(scroll?: boolean) {
    this.ngxSpinnerService.show();

    if (this.secretKey) {
      this.isSharedView = true;
      this.studyShared = await this.studiesService
        .getStudyShared(this.secretKey)
        .toPromise();
      this.publications = this.studyShared.studies;
    } else {
      const publicationNew = await this.studiesService
        .getPublications(
          this.myPage,
          this.pageSize,
          this.orderType,
          this.checkboxesId,
          this.resultTerm
        )
        .toPromise();

      if (scroll) {
        this.publications.push(...publicationNew);
      } else {
        this.publications = publicationNew;
      }

      this.publicationsCopy = [...this.publications];
    }

    this.ngxSpinnerService.hide();
  }

  openPublicationDetail(publication: Publications) {
    if (!publication.hasAccess) {
      publication.message = applyLink(publication.message);
      this.modalRef = this.bsModalService.show(BlockedContentComponent, {
        class: "modal-lg",
        initialState: { publication },
      });
    } else if (publication.studyType === StudyTypeENUM.File) {
      this.studiesService.downloadFileByStudyId(publication);
    } else if (publication.studyType === StudyTypeENUM.Embed) {
      this.modalRef = this.bsModalService.show(PublicationDetailComponent, {
        class: "modal-lg",
        initialState: { isSharedView: this.isSharedView, publication },
      });

      this.modalRef.content.publicationEmitter.subscribe(
        (res) => {
          this.getShareKey(res);
        },
        (err) => {
          return false;
        }
      );
    }

    this.ngxSpinnerService.hide();
  }

  sharePublication(publication: Publications) {
    this.publicationShare = publication;
    this.getShareKey(publication.id);
  }

  convertEnumToArray() {
    const arrayObjects = [];
    let i = 1;
    for (const [propertyKey, propertyValue] of Object.entries(OrderTypeEnum)) {
      if (!Number.isNaN(Number(propertyKey))) {
        continue;
      }
      arrayObjects.push({ id: i, title: propertyValue });
      i++;
    }

    return arrayObjects;
  }

  changeSelectOrderType(event: any) {
    this.orderType = event.id;
    this.getAllStudies();
  }

  changeWithPermissions(event) {
    this.ngxSpinnerService.show();
    this.withPermission = event;

    this.publications = this.withPermission
      ? this.publications.filter((f) => f.hasAccess === true)
      : this.publicationsCopy;

    this.ngxSpinnerService.hide();
  }

  onChangeSearch(eventQuery: string) {
    this.query = eventQuery;
    if (eventQuery.length >= 3 || eventQuery.length === 0) {
      this.getAllStudies();
    }
  }

  getShareKey(publicationId): void {
    this.shareURL = null;

    this.commonService
      .getShareKey({
        moduleId: Modules.studiesPublications,
        moduleSubItemId: null,
        columnsExcluded: null,
        parameters: {
          publicationId: publicationId,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}estudos-publicacoes/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
      });
  }

  async onSendEmail(): Promise<void> {
    await this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .toPromise();

    this.ngxSpinnerService.hide();
  }

  onPutEmail(event) {
    this.email = event;
  }
}
