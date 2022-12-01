import {
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
  TemplateRef,
} from "@angular/core";

import locales from "@/locales/positioning";
import { IDefault } from "@/shared/interfaces/positions";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";

@Component({
  selector: "app-filter-by",
  templateUrl: "./filter-by.component.html",
  styleUrls: ["./filter-by.component.scss"],
})
export class FilterByComponent implements OnInit {
  @Input() label: string = locales.showTo;

  @Input() list: IDefault[];
  @Input() selected: IDefault;
  @Output() selectEvent = new EventEmitter<IDefault>();

  public internalSelected: IDefault = null;

  public locales = locales;

  public modalRef?: BsModalRef;

  constructor(private modalService: BsModalService) {}

  ngOnInit(): void {
    this.internalSelected = this.selected;
  }

  openModal(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  changeFilterBy(event) {
    this.internalSelected = event;
  }

  filterAndClose() {
    this.selectEvent.emit(this.internalSelected);
    this.modalRef?.hide();
  }
}
