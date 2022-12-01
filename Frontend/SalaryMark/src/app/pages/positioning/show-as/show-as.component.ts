import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  Output,
  Input,
  EventEmitter,
  ViewChild,
} from "@angular/core";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import locales from "@/locales/positioning";
import { TooltipDirective } from "ngx-bootstrap/tooltip";
import { FormGroup } from "@angular/forms";

@Component({
  selector: "app-show-as",
  templateUrl: "./show-as.component.html",
  styleUrls: ["./show-as.component.scss"],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class ShowAsComponent implements OnInit {
  @ViewChild("popLink") popLink: TooltipDirective;

  @Input() form: FormGroup;
  firstBlock: FormGroup;
  secondBlock: FormGroup;

  locales = locales;

  public inputModalShow: IDialogInput;
  public description: string;
  public isOpenSelect: boolean;
  public subject: string;

  constructor() {}

  ngOnInit(): void {
    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModalShowAs",
      title: locales.showConfiguration,
      btnPrimaryTitle: "",
      btnSecondaryTitle: "",
      isInfoPosition: true,
      isRightModal: true,
    };
  }

  ngOnChanges() {
    if (this.form && this.form.get("items")["controls"].length > 0) {
      this.calculateBlock();
    }
  }

  // showLine(){
  //   return this.checkedShow && this.checkedShow.length > 6;
  // }

  chunk = (arr, size) =>
    arr.reduce(
      (acc, e, i) => (
        i % size ? acc[acc.length - 1].push(e) : acc.push([e]), acc
      ),
      []
    );

  calculateBlock() {
    const size = this.form.get("items")["controls"].length / 2;
    const blocks = this.chunk(
      this.form.get("items")["controls"],
      Math.ceil(size)
    );
    this.firstBlock = blocks ? blocks[0] : [];
    this.secondBlock = blocks && blocks.length === 2 ? blocks[1] : [];
  }
}
