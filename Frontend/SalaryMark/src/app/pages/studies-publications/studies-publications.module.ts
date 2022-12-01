import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { StudiesPublicationsComponent } from "./studies-publications.component";
import { SharedModule } from "@/shared/shared.module";
import { StudiesPublicationRoutingModule } from "./studies-publications-routing.module";
import { ScrollService } from "@/shared/services/scroll/scroll.service";

@NgModule({
  declarations: [StudiesPublicationsComponent],
  imports: [CommonModule, SharedModule, StudiesPublicationRoutingModule],
  providers: [ScrollService],
})
export class StudiesPublicationsModule {}
