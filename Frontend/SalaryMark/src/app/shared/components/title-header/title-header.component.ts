import { Component, OnInit, Input } from "@angular/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-title-header",
  templateUrl: "./title-header.component.html",
  styleUrls: ["./title-header.component.scss"],
})
export class TitleHeaderComponent implements OnInit {
  @Input() title: string;
  @Input() customClass: string;
  @Input() hasBack: boolean;

  public hasHistory: boolean;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.hasHistory = this.router.navigated;
  }

  backPage() {
    if (this.hasHistory) {
      history.go(-1);
    }
  }
}
