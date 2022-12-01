import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { TooltipComponent } from "./tooltip.component";
import { RouterModule } from "@angular/router";

@NgModule({
  declarations: [TooltipComponent],
  imports: [CommonModule, RouterModule],
})
export class TooltipModule {}
