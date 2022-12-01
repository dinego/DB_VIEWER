import { Component, Input, OnInit } from "@angular/core";
import { IDescriptionPosition } from "../position-detail-tab/common/positioin-detail";

@Component({
  selector: "app-lobbu-tab",
  templateUrl: "./lobbu-tab.component.html",
  styleUrls: ["./lobbu-tab.component.scss"],
})
export class LobbuTabComponent implements OnInit {
  @Input() descriptionPosition: IDescriptionPosition;

  constructor() {}

  ngOnInit(): void {}
}
