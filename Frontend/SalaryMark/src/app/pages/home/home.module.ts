import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SharedModule } from "@/shared/shared.module";

import { HomeRoutingModule } from "./home-routing.module";
import { HomeComponent } from "./home.component";
import { ConfirmationComponent } from "./confirmation/confirmation.component";
import { NotificationDetailComponent } from "./notification-detail/notification-detail.component";
import { VideoDetailComponent } from "./video-detail/video-detail.component";

import { SwiperModule } from "swiper/angular";
import { SliderCardComponent } from "./slider-card/slider-card.component";
import { AuthService } from "@/shared/services/auth/auth.service";
import { UserService } from "@/shared/services/user/user.service";

@NgModule({
  declarations: [
    HomeComponent,
    ConfirmationComponent,
    NotificationDetailComponent,
    VideoDetailComponent,
    SliderCardComponent,
  ],
  imports: [CommonModule, HomeRoutingModule, SharedModule, SwiperModule],
  providers: [AuthService, UserService],
})
export class HomeModule {}
