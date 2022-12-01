import locales from "@/locales/common";

import { Component, Input, OnInit } from "@angular/core";

@Component({
  selector: "app-button-secundary",
  templateUrl: "./button-secundary.component.html",
  styleUrls: ["./button-secundary.component.scss"],
})
export class ButtonSecundaryComponent implements OnInit {
  @Input() label: string;

  @Input() disabled: boolean;

  public locales = locales;
  constructor() {}

  ngOnInit(): void {}
}
