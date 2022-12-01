import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import locales from "@/locales/common";

@Component({
  selector: "app-button-primary",
  templateUrl: "./button-primary.component.html",
  styleUrls: ["./button-primary.component.scss"],
})
export class ButtonPrimaryComponent implements OnInit {
  @Input() label: string;
  @Input() disabled: boolean;

  public locales = locales;

  constructor() {}

  ngOnInit(): void {}
}
