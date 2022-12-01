import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  Input,
  ViewChild,
} from "@angular/core";

import locales from "@/locales/menu";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { IUser } from "@/shared/models/token";
import { UserService } from "@/shared/services/user/user.service";

const Constants = {
  classEmptyPhoto: "empty-photo",
  classFullPhoto: "full-photo",
  emptyPhoto: null,
};

interface TypePhoto {
  class: string;
  url: string | ArrayBuffer;
}

@Component({
  selector: "app-profile-picture",
  templateUrl: "./profile-picture.component.html",
  styleUrls: ["./profile-picture.component.scss"],
})
export class ProfilePictureComponent implements OnInit {
  @Input() user: IUser;
  @Input() initials: string;

  @Output() removePhoto = new EventEmitter();

  @Output() setUserPhoto = new EventEmitter<string>();

  @Output() updatePhoto = new EventEmitter<FormData>();
  @ViewChild("attachments") attachment: any;
  public constants = Constants;
  public files: any[];
  public isPhoto: boolean;
  public inputModalShow: IDialogInput;
  public locales = locales;
  public typePhoto: TypePhoto;
  userPhoto: string;
  name: string;

  constructor(private userService: UserService) {
    this.files = [];
  }

  async ngOnInit() {
    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModalProfilePhoto",
      title: locales.myAccount,
      btnPrimaryTitle: locales.save,
      btnSecondaryTitle: locales.cancel,
    };
    this.userPhoto = await this.userService.retrieveUserPhoto();
    if (this.userPhoto && this.userPhoto.includes("empty-profile")) {
      this.userPhoto = "";
    }
    this.getProfilePhoto();
  }

  getProfilePhoto() {
    if (this.userPhoto) {
      this.onPhotoUpdate(this.userPhoto);
    } else {
      this.onPhotoRemove();
    }
  }

  get getChangePhoto(): string {
    return this.isPhoto ? "../../../../assets/imgs/svg/remove-photo.svg" : "";
  }

  onPhotoRemove() {
    this.isPhoto = false;
    this.files = [];
    this.typePhoto = {
      class: this.constants.classEmptyPhoto,
      url: this.constants.emptyPhoto,
    };
    this.removePhoto.emit();
  }

  onPhotoUpdate(url: string | ArrayBuffer) {
    this.isPhoto = true;
    this.typePhoto = {
      class: this.constants.classFullPhoto,
      url,
    };
  }

  onPhotoChanged(event: any) {
    if (event.target.files && event.target.files[0]) {
      const reader = new FileReader();
      this.files = event.target.files;
      reader.readAsDataURL(event.target.files[0]);

      // tslint:disable-next-line: no-shadowed-variable
      reader.onload = (event) => {
        this.onPhotoUpdate(event.target.result);
      };
    } else {
      this.onPhotoRemove();
    }
  }

  onSave() {
    if (this.files.length > 0 || this.typePhoto.url !== this.userPhoto) {
      const formData = new FormData();
      for (const file of this.files) {
        formData.append("attachment", file, file.name);
      }
      this.setUserPhoto.emit(this.files[0]);
      this.updatePhoto.emit(formData);
    }
  }
}
