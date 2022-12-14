import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Pipe({
  name: 'highlightsCase'
})
export class HighlightsPipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(value: any, args: any): any {
    if (!args) {
      return value;
    }
    // Match in a case insensitive maneer
    const re = new RegExp(args, 'gi');
    if(typeof value == 'boolean')
      return value;

      const match = value?.match(re);

    // If there's no match, just return the original value.
    if (!match) {
      return value;
    }

    const replacedValue = value.replace(re, '<mark>' + match[0] + '</mark>');
    return this.sanitizer.bypassSecurityTrustHtml(replacedValue);
  }
}
