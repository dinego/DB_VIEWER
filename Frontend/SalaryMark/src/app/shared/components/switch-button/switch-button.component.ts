import {
  Component,
  OnInit,
  Output,
  EventEmitter,
  AfterViewInit,
  forwardRef,
  Input,
  OnChanges,
  SimpleChanges,
} from "@angular/core";
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR,
} from "@angular/forms";

@Component({
  selector: "app-switch-button",
  templateUrl: "./switch-button.component.html",
  styleUrls: ["./switch-button.component.scss"],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SwitchButtonComponent),
      multi: true,
    },
  ],
})
export class SwitchButtonComponent
  implements AfterViewInit, OnInit, ControlValueAccessor, OnChanges
{
  @Output() expandAllRows = new EventEmitter<boolean>();
  @Input() isChecked: boolean = false;
  @Input() disabled: boolean = false;
  @Input() SliderRed: boolean = false;
  @Input() SliderBlue: boolean = false;
  @Input() SliderGreen: boolean = false;

  public formControl = new FormControl(false);

  constructor() {}
  ngOnChanges(changes: SimpleChanges): void {
    if (changes.isChecked)
      this.formControl.setValue(changes.isChecked.currentValue);
  }

  ngOnInit(): void {
    this.formControl.setValue(this.isChecked);
  }

  ngAfterViewInit() {
    this.formControl.valueChanges.subscribe((value) => {
      this.onChangeFn(value);
    });
  }

  invertValue() {
    const newValue = !this.formControl.value;
    this.writeValue(newValue);
    this.expandAllRows.emit(newValue);
  }

  writeValue(value: boolean): void {
    this.formControl.setValue(value);
  }

  registerOnChange(fn: (_: any) => void): void {
    this.onChangeFn = fn;
  }

  registerOnTouched(fn: any): void {}

  onChangeFn = (_: any) => {};
}
