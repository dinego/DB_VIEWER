import { Injectable } from "@angular/core";
import { BehaviorSubject } from "rxjs";

@Injectable()
export class ScrollService {
  public scrollSubject = new BehaviorSubject(false);

  constructor() {}

  callScroll(doScroll: boolean) {
    this.scrollSubject.next(doScroll);
  }
}
