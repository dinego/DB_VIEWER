import { ToastrModule, ToastrService } from "ngx-toastr";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SharedModule } from "@/shared/shared.module";
import { UserService } from "@/shared/services/user/user.service";
import { MenuComponent } from "@/core/menu/menu.component";
import { MenuHeaderComponent } from "@/core/menu-header/menu-header.component";
import { ProfilePictureComponent } from "@/core/components/profile-picture/profile-picture.component";

import { CoreComponent } from "./core.component";
import { ChangePasswordComponent } from "./components/change-password/change-password.component";
import { ContactUsComponent } from "./components/contact-us/contact-us.component";
import { TokenService } from "@/shared/services/token/token.service";

@NgModule({
  declarations: [
    CoreComponent,
    MenuComponent,
    MenuHeaderComponent,
    ProfilePictureComponent,
    ChangePasswordComponent,
    ContactUsComponent,
  ],
  exports: [CoreComponent],
  imports: [CommonModule, SharedModule, ToastrModule.forRoot()],
  providers: [TokenService, UserService, ToastrService],
})
export class CoreModule {}
