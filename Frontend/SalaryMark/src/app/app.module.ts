import { BrowserModule } from "@angular/platform-browser";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { LOCALE_ID, NgModule } from "@angular/core";
import { registerLocaleData } from "@angular/common";
import ptBr from "@angular/common/locales/pt";
import { RouterModule } from "@angular/router";
import { HttpClientModule } from "@angular/common/http";

import { CoreModule } from "@/core/core.module";
import { SharedModule } from "@/shared/shared.module";
import { HomeModule } from "@/pages/home/home.module";
import { SalaryTableModule } from "@/pages/salary-table/salary-table.module";
import { DashboardModule } from "@/pages/dashboard/dashboard.module";
import { PositioningModule } from "@/pages/positioning/positioning.module";
import { PositionsModule } from "@/pages/positions/positions.module";

import { appRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { HttpModule } from "./shared/http/http.module";
import { MyReportsModule } from "./pages/my-reports/my-reports.module";
import { CookieService } from "ngx-cookie-service";
import { StudiesPublicationsModule } from "./pages/studies-publications/studies-publications.module";

registerLocaleData(ptBr);

@NgModule({
  declarations: [AppComponent],
  imports: [
    appRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    HomeModule,
    DashboardModule,
    PositioningModule,
    RouterModule,
    SalaryTableModule,
    MyReportsModule,
    StudiesPublicationsModule,
    PositionsModule,
    SharedModule,
    RouterModule,
    HttpModule,
    HttpClientModule,
  ],
  providers: [{ provide: LOCALE_ID, useValue: "pt-BR" }, CookieService],
  bootstrap: [AppComponent],
})
export class AppModule {}
