import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { TooltipModule } from "ngx-bootstrap/tooltip";

import { SharedModule } from "@/shared/shared.module";

import { PositionsRoutingModule } from "./positions-routing.module";
import { PositionsComponent } from "./positions.component";
import { ListComponent } from "./list/list.component";

import { PositionListService } from "@/shared/services/position-table/position-table.service";
import { MapComponent } from "./map/map.component";
import { BsDropdownModule } from "ngx-bootstrap/dropdown";
import { DndModule } from "ngx-drag-drop";
import { TableDragAndDropComponent } from './map/components/table-drag-and-drop/table-drag-and-drop.component';

@NgModule({
  declarations: [PositionsComponent, ListComponent, MapComponent, TableDragAndDropComponent],
  imports: [
    SharedModule,
    CommonModule,
    PositionsRoutingModule,
    DndModule,
    TooltipModule.forRoot(),
    BsDropdownModule.forRoot(),
  ],
  providers: [PositionListService],
})
export class PositionsModule {}
