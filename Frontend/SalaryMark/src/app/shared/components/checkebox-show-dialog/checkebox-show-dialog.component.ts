import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
} from "@angular/core";

import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { CheckShowItem } from "@/shared/models/positioning";
import locales from "@/locales/common";

@Component({
  selector: "app-checkebox-show-dialog",
  templateUrl: "./checkebox-show-dialog.component.html",
  styleUrls: ["./checkebox-show-dialog.component.scss"],
})
export class CheckeboxShowDialogComponent implements OnInit {
  @Input() checkedShow: Array<CheckShowItem>;
  @Input() inputModalShow: IDialogInput;

  @Output() sendChecked = new EventEmitter<Array<CheckShowItem>>();
  public firstBlock: Array<CheckShowItem> = [];
  public secondBlock: Array<CheckShowItem> = [];

  public locales = locales;

  constructor() {}

  ngOnInit(): void {
    this.calculateBlock();
  }

  changeCheckedBox(value: boolean, id: string) {
    this.checkedShow.forEach((item) => {
      if (item.id === id) {
        item.checked = value;
      }
    });
    this.sendChecked.emit(this.checkedShow);
  }
  public trackItemModal(index: number, item: CheckShowItem) {
    return item.id;
  }
  showLine() {
    return this.checkedShow && this.checkedShow.length > 6;
  }

  calculateBlock() {
    const size = this.checkedShow.length / 2;
    const blocks = this.chunk(this.checkedShow, Math.ceil(size));
    this.firstBlock = blocks ? blocks[0] : [];
    this.secondBlock = blocks && blocks.length === 2 ? blocks[1] : [];
  }

  resetFilters() {
    this.firstBlock.forEach((value) => {
      value.checked = true;
      this.changeCheckedBox(value.checked, value.id);
    });

    this.secondBlock.forEach((value) => {
      value.checked = true;
      this.changeCheckedBox(value.checked, value.id);
    });
  }

  saveAndCloseModal() {}

  chunk = (arr, size) =>
    arr.reduce(
      (acc, e, i) => (
        i % size ? acc[acc.length - 1].push(e) : acc.push([e]), acc
      ),
      []
    );
}
