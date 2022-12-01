import { SendLinkAccess } from "./../../../../shared/models/user-parameter";
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  Output,
  Input,
  EventEmitter,
  ViewChild,
  OnChanges,
  SimpleChanges,
} from "@angular/core";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import locales from "@/locales/parameters";
import { TooltipDirective } from "ngx-bootstrap/tooltip";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ParamsService } from "@/shared/services/params/params.service";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-send-link-access",
  templateUrl: "./send-link-access.component.html",
  styleUrls: ["./send-link-access.component.scss"],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class SendLinkAccessComponent implements OnInit, OnChanges {
  @ViewChild("popLink") popLink: TooltipDirective;

  @Input() data: any;

  locales = locales;

  public inputModalShow: IDialogInput;
  public description: string;
  public isOpenSelect: boolean;
  public subject: string;
  public sendLink: SendLinkAccess = new SendLinkAccess();

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private paramsService: ParamsService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModalSendLinkAccess",
      btnWithoutCancel: false,
      title: locales.sendAccessLink,
      btnPrimaryTitle: locales.submit,
      btnSecondaryTitle: locales.cancel,
    };

    this.form = this.fb.group({
      message: this.fb.control(null, [Validators.required]),
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.data) {
      setTimeout(() => {
        this.popLink.show();
      }, 500);
    }
  }

  async sendEmailToUser() {
    this.sendLink.to = this.data.userParameter.mail;

    this.sendLink.url = this.data.accessLink;

    this.sendLink.message = this.form.getRawValue();

    if (this.form.valid) {
      await this.paramsService.sendLinkAccess(this.sendLink).toPromise();
      this.toastrService.success(
        locales.sendSucessMessage,
        locales.saveSucessMessageTitle
      );
      return;
    }
    this.toastrService.error(locales.invalidForm, locales.errorTitleForm);
  }
}
