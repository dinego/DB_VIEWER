import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  TemplateRef,
} from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";

@Component({
  selector: "app-modal-chart",
  templateUrl: "./modal-chart.component.html",
  styleUrls: ["./modal-chart.component.scss"],
})
export class ModalChartComponent implements OnInit {
  public modalRef?: BsModalRef;
  public showModal = false;

  @Output() reflow = new EventEmitter<boolean>();

  constructor(private modalService: BsModalService) {}

  ngOnInit(): void {}

  openModal(template: TemplateRef<any>) {
    this.showModal = true;
    this.modalRef = this.modalService.show(
      template,
      Object.assign({}, { class: "modal-xl" })
    );
    this.reflow.emit(true);
  }
  onCloseModal() {
    this.showModal = false;
  }
}
