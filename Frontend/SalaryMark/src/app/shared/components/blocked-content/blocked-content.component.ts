import { Publications } from "@/shared/services/studies/common/publications";
import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";

@Component({
  selector: "app-blocked-content",
  templateUrl: "./blocked-content.component.html",
  styleUrls: ["./blocked-content.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BlockedContentComponent implements OnInit {
  publication: Publications;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit(): void {}
}
