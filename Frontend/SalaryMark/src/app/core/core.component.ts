import { Component, OnInit } from "@angular/core";
@Component({
  selector: "app-core",
  templateUrl: "./core.component.html",
  styleUrls: ["./core.component.scss"],
})
export class CoreComponent implements OnInit {
  public showMenuControl: boolean;
  public paddingRight = 16;
  public paddingLeft = 16;

  constructor() {}

  ngOnInit(): void {
    this.changeViewPort();
    setTimeout(() => {
      this.calculatePaddingHorizontally();
    }, 10);
  }

  changeViewPort() {
    var mvp = document.getElementById("myViewport");
    const forcedMobileView = JSON.parse(localStorage.getItem("forceMobile"));
    if (!forcedMobileView) {
      if (screen.width > 768) {
        mvp.setAttribute("content", "width=1210, user-scalable=yes");
      } else {
        mvp.setAttribute("content", "width=device-width, initial-scale=1");
      }
    }
  }

  setOpenMenu(event: boolean): void {
    this.showMenuControl = event;
  }

  calculatePaddingHorizontally() {
    if (window.innerWidth > 1800) {
      const leftover = window.innerWidth - 1400;

      const eachSidePadding = leftover / 2;
      this.paddingLeft = eachSidePadding;
      this.paddingRight = eachSidePadding;
    } else {
      this.paddingLeft = 16;
      this.paddingRight = 16;
    }
  }
}
