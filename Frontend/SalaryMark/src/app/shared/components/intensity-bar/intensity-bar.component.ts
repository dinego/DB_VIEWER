import {
  Component,
  ElementRef,
  Input,
  OnInit,
  Renderer2,
  ViewChild,
} from "@angular/core";

@Component({
  selector: "app-intensity-bar",
  templateUrl: "./intensity-bar.component.html",
  styleUrls: ["./intensity-bar.component.scss"],
})
export class IntensityBarComponent implements OnInit {
  @ViewChild("progress") progress: ElementRef;

  @Input() percent: number;
  @Input() min: number;
  @Input() max: number;

  public percentToView: number;
  public halfRed: number = 60;
  public red: number = 30;

  private afterInit: boolean;

  constructor(private renderer: Renderer2, elRef: ElementRef) {}

  ngOnInit(): void {
    this.setPercentValue();
  }

  ngOnChanges(event) {
    this.setPercentValue();
    if (this.afterInit) this.setStyleProgressColor();
  }

  ngAfterViewInit() {
    this.setStyleProgressColor();
    this.afterInit = true;
  }

  setPercentValue() {
    const basePercent = this.percent / this.min;
    this.percentToView = ((this.percent / this.max) * 100) / 2 + basePercent;
  }

  setStyleProgressColor() {
    const half = Math.round(((this.min / this.max) * 100) / 3).toString();
    const half2 = Math.round((this.min / this.max) * 100 * 2).toString();

    const el = this.progress.nativeElement;
    this.renderer.setStyle(
      el,
      "background",
      `transparent linear-gradient(90deg, #ddb1a8 0%, #d48e84 ${half}%, #84b160 ${half2}%, #54934f 100%)`
    );
  }
}
