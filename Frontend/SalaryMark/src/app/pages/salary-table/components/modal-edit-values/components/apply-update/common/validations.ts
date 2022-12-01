import { AbstractControl } from "@angular/forms";

export class UpdateValidation {
  static CompareGsms(control: AbstractControl) {
    const gsmInitial = control.get("gsmInitial").value;
    const gsmFinal = control.get("gsmFinal").value;

    if (gsmFinal >= gsmInitial) return null;

    control.get("gsmFinal").setErrors({ gmsFinalMinor: true });
  }
}
