import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";

@Component({
  selector: "app-search",
  templateUrl: "./search.component.html",
  styleUrls: ["./search.component.scss"],
})
export class SearchComponent implements OnInit {
  @Input() text: String;
  @Input() isFullSize: boolean;
  @Output() searchQuery = new EventEmitter();

  constructor() {}

  ngOnInit(): void {}
}
