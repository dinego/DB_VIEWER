import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-radio-button",
  templateUrl: "./radio-button.component.html",
  styleUrls: ["./radio-button.component.scss"],
})
export class RadioButtonComponent implements OnInit {
  @Input() label: string;
  @Input() itemId: number;
  @Input() checked: boolean;

  @Output() checkEvent = new EventEmitter<number>();

  constructor() {}

  ngOnInit(): void {}

  selectItem() {
    this.checkEvent.emit(this.itemId);
  }
}
