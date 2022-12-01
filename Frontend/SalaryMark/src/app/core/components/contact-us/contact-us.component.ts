import { Component, OnInit, Output, EventEmitter } from "@angular/core";

import locales from "@/locales/menu";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { ContactUs } from "@/shared/models/contact-us";

@Component({
  selector: "app-contact-us",
  templateUrl: "./contact-us.component.html",
  styleUrls: ["./contact-us.component.scss"],
})
export class ContactUsComponent implements OnInit {
  @Output() send = new EventEmitter<ContactUs>();

  public inputModalShow: IDialogInput;
  public message: string;
  public isOpenSelect: boolean;
  public locales = locales;
  public selected = locales.doubts;
  public subject: string;
  public attachment: File;

  constructor() {}

  ngOnInit(): void {
    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModalContactUs",
      title: locales.sendMessage,
      btnPrimaryTitle: locales.submit,
      btnSecondaryTitle: locales.cancel,
    };
  }

  onSelect() {
    this.isOpenSelect = !this.isOpenSelect;
  }

  onSelected(option: string) {
    this.selected = option;
    this.isOpenSelect = false;
  }

  chooseFile() {
    document.getElementById("file").click();
  }

  removeFile() {
    this.attachment = null;
    // tslint:disable-next-line: no-string-literal
    document.getElementById("file")["value"] = null;
  }

  setFileToUpload(files: FileList) {
    if (files.length > 0) {
      this.attachment = files.item(0);
    }
  }

  onSave() {
    if (this.message && this.subject) {
      const params: ContactUs = {
        attachment: this.selected,
        subject: this.subject,
        message: this.message,
      };
      this.send.emit(params);
    }
  }
}
