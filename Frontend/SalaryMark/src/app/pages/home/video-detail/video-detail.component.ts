import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { VideoItem } from '../../../shared/models/youtube/videos/item';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-video-detail',
  templateUrl: './video-detail.component.html',
  styleUrls: ['./video-detail.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class VideoDetailComponent implements OnInit {
  public videoItem: VideoItem;

  constructor(
    public bsModalRef: BsModalRef,
    private domSanitizer: DomSanitizer
  ) { }

  ngOnInit() { }

  get domSanitizedVideoItem() {
    let videoString = this.videoItem.snippet.resourceId ? this.videoItem.snippet.resourceId.videoId : this.videoItem.id;

    return this.domSanitizer.bypassSecurityTrustResourceUrl(

      `https://www.youtube.com/embed/${videoString}`
    );
  }
}
