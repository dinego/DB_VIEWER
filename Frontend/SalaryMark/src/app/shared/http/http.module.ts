import { NgModule, Optional, SkipSelf } from '@angular/core';
import {
  HttpClient,
  HttpClientModule
} from '@angular/common/http';
import { HttpService } from './http.service';

@NgModule({
  imports: [HttpClientModule],
  providers: [
    {
      provide: HttpClient,
      useClass: HttpService
    }
  ]
})
export class HttpModule {
  constructor(@Optional() @SkipSelf() parentModule: HttpModule) {
    // Import guard
    if (parentModule) {
      throw new Error(
        `${parentModule} has already been loaded. Import Core module in the AppModule only.`
      );
    }
  }
}
