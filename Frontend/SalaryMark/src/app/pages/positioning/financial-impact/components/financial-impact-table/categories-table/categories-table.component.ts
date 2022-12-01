import { Categories } from "@/shared/models/positioning";
import { Component, Input, OnInit } from "@angular/core";

@Component({
  selector: "app-categories-table",
  templateUrl: "./categories-table.component.html",
  styleUrls: ["./categories-table.component.scss"],
})
export class CategoriesTableComponent implements OnInit {
  @Input() categories: Array<Categories>;
  @Input() filterBy: string = "Perfil";

  constructor() {}

  ngOnInit(): void {}
}
