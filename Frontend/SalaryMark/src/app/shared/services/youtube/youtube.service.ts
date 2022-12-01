import { Injectable } from "@angular/core";
import { HttpClient, HttpParams, HttpBackend } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { Observable } from "rxjs";
import { VideosList } from "../../../shared/models/youtube/videos/list";

@Injectable({
  providedIn: "root",
})
export class YoutubeService {
  private httpClient: HttpClient;

  constructor(handler: HttpBackend) {
    this.httpClient = new HttpClient(handler);
  }

  public getVideosFromPlaylist(data: any): Observable<VideosList> {
    const params = new HttpParams({
      fromObject: { ...data, key: environment.youtube.key },
    });

    return this.httpClient.get<VideosList>(
      `${environment.youtube.api}${environment.youtube.playlists}`,
      {
        params,
      }
    );
  }

  public getVideoWelcome(): Observable<VideosList> {
    let data: any = {
      part: "snippet",
      id: "liEAh25xFlQ",
    };

    const params = new HttpParams({
      fromObject: { ...data, key: environment.youtube.key },
    });

    return this.httpClient.get<VideosList>(
      `${environment.youtube.api}${environment.youtube.videos}`,
      {
        params,
      }
    );
  }
}
