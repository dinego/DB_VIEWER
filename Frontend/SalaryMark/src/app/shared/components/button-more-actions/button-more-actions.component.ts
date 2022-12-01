import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import locales from "@/locales/common";
import { trigger, transition, animate, style } from "@angular/animations";

@Component({
  selector: "app-button-more-actions",
  templateUrl: "./button-more-actions.component.html",
  styleUrls: ["./button-more-actions.component.scss"],
  animations: [
    trigger("inOutAnimation", [
      transition(":enter", [
        // style({ width: 0, opacity: 0 }),
        // animate('0.5s ease-out', style({ width: 510, opacity: 1 })),
      ]),
      transition(":leave", [
        // style({ width: 510, opacity: 1 }),
        // animate('0.5s ease-in', style({ width: 0, opacity: 0 })),
      ]),
    ]),
  ],
})
export class ButtonMoreActionsComponent implements OnInit {
  @Input() isHelp: boolean;
  @Input() isMore: boolean;
  @Input() isShare: boolean;
  @Input() text: string;
  @Output() onShareClick = new EventEmitter();

  public isExpand: boolean;
  public locales = locales;

  constructor() {}

  ngOnInit(): void {
    if (!this.isMore) {
      this.clickExpandAction();
    }
  }

  get isDetailsExpand(): string {
    return this.isExpand
      ? "fa-arrow-alt-circle-right"
      : "fa-arrow-alt-circle-left";
  }

  clickExpandAction() {
    this.isExpand = !this.isExpand;
  }

  onClickShare(): void {}
}
