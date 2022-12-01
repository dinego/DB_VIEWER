import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { DndDropEvent, DropEffect } from "ngx-drag-drop";
import { IDragData } from "./common/drag-data";

@Component({
  selector: "app-table-drag-and-drop",
  templateUrl: "./table-drag-and-drop.component.html",
  styleUrls: ["./table-drag-and-drop.component.scss"],
})
export class TableDragAndDropComponent implements OnInit {
  @Input() header: any;
  @Input() rows: any;
  @Input() editable: boolean;
  @Input() query: string;

  @Output() clickItemEmitter = new EventEmitter<number>();
  @Output() saveEmitter = new EventEmitter<any>();
  @Output() cancelEmitter = new EventEmitter<void>();

  public validatorColRow: {
    row: number;
    col: number;
    data: any;
    list: any[];
    index;
  };

  constructor() {}

  async ngOnInit() {}

  onDragStart(event: DragEvent) {}

  onDragEnd(event: DragEvent) {}

  onDragover(event: DragEvent) {}

  onDrop(event: DndDropEvent, item?: any, rowIndex?: number) {
    const list = item?.data;

    if (list && (event.dropEffect === "copy" || event.dropEffect === "move")) {
      let index = event.index;

      if (typeof index === "undefined") {
        index = list?.length;
      }

      list.splice(index, 0, event.data);

      this.validatorColRow = {
        col: item.colPos,
        row: rowIndex,
        data: item.data,
        list: list,
        index: index,
      };
    }
  }

  onDragged(item: any, row: number, data: any, effect: DropEffect) {
    if (effect === "move") {
      const rowsArray = Object.values(this.rows[row]);
      const objManipulator: any = rowsArray.find(
        (f: any) => f.colPos === item.colPos
      );

      const columnItemRemove = this.rows[row].indexOf(objManipulator);
      const rowItemRemove = row;

      Object.values(this.rows[rowItemRemove][columnItemRemove].data).forEach(
        (f: any, index) => {
          if (
            f.value === data.value &&
            this.validatorColRow.data !==
              this.rows[rowItemRemove][columnItemRemove].data
          ) {
            this.removeItemColRow(columnItemRemove, rowItemRemove, index);
          }
        }
      );
    }
  }

  removeItemColRow(col: number, row: number, index: number) {
    this.rows[row][col].data.splice(index, 1);
  }

  clickItem(event) {
    this.clickItemEmitter.emit(event.positionSMId);
  }

  clickSave() {
    this.saveEmitter.emit(this.rows);
  }

  clickCancel() {
    this.cancelEmitter.emit();
  }
}
