import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  Input,
  forwardRef,
  AfterViewInit,
  Output,
  EventEmitter,
} from "@angular/core";
import {
  NG_VALUE_ACCESSOR,
  ControlValueAccessor,
  FormControl,
} from "@angular/forms";

@Component({
  selector: "app-checkbox",
  templateUrl: "./checkbox.component.html",
  styleUrls: ["./checkbox.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CheckboxComponent),
      multi: true,
    },
  ],
})
export class CheckboxComponent
  implements AfterViewInit, OnInit, ControlValueAccessor
{
  @Input() label: string;
  @Input() disabled: boolean = false;
  @Input() changeItem?: any = null;
  @Input() id: number;
  @Input() children: boolean = false;
  @Input() checked: boolean;
  @Output() selected = new EventEmitter<boolean>();
  @Output() selectedAll = new EventEmitter<boolean>();
  public formControl = new FormControl(false);
  randomId = "";
  constructor() {}

  ngOnInit(): void {
    this.randomId = this.generateId();
  }

  ngAfterViewInit() {
    this.formControl.valueChanges.subscribe((value) => {
      this.onChangeFn(value);
    });
  }

  ngOnChanges() {
    if (this.changeItem != null) {
      this.formControl.setValue(this.changeItem);
    }
  }

  invertValue() {
    this.writeValue(!this.formControl.value);
  }

  writeValue(value: boolean): void {
    this.formControl.setValue(value);
    if (this.children) {
      this.selected.emit(value);
      return;
    }
    this.selectedAll.emit(value);
  }

  registerOnChange(fn: (_: any) => void): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {}

  onChangeFn = (_: any) => {};

  generateId(): string {
    return "c-" + Math.random().toString(16).slice(2);
  }
}
