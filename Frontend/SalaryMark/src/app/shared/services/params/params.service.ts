import { SendLinkAccess } from "./../../models/user-parameter";
import {
  DisplaySettins,
  DisplayTypesConfigurations,
  DisplayTypeSettings,
} from "@/shared/models/display-settings";
import {
  GlobalLabels,
  PreferenceDisplay,
} from "@/shared/models/preferences-display";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { environment } from "src/environments/environment";
@Injectable({
  providedIn: "root",
})
export class ParamsService {
  constructor(private httpClient: HttpClient) {}

  getDisplayConfiguration(): Observable<DisplayTypesConfigurations> {
    return this.httpClient.get<DisplayTypesConfigurations>(
      environment.api.displaySettins.getDisplayConfiguration
    );
  }

  updateDisplaySettings(
    displaySettings: DisplayTypeSettings[]
  ): Observable<any> {
    return this.httpClient.put<any>(
      environment.api.displaySettins.updateDisplayConfiguration,
      displaySettings
    );
  }

  updateGlobalLabels(globalLabels: GlobalLabels[]): Observable<any> {
    return this.httpClient.put<any>(
      environment.api.displaySettins.updateGlobalLabels,
      globalLabels
    );
  }

  sendLinkAccess(sendLinkAccess: SendLinkAccess): Observable<string> {
    return this.httpClient.put<string>(environment.api.home.sendLinkAccess, {
      sendLinkAccess,
    });
  }
}
