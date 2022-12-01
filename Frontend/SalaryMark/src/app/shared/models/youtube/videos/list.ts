import { VideoItem } from './item';

export interface VideosList {
  kind: string;
  etag: string;
  nextPageToken: string;
  pageInfo: PageInfo;
  items: VideoItem[];
}

export interface PageInfo {
  totalResults: number;
  resultsPerPage: number;
}
