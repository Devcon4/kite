import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';

export type Link = {
  group: string;
  path: string;
  name: string;
  namespaceProperty: string;
  kind: string;
  tags: string[];
  description: string;
};

type LinkResponse = {
  list: Link[];
  count: number;
};

@Injectable({ providedIn: 'root' })
export class LinkState {
  links = new BehaviorSubject<Link[]>([]);

  constructor(private httpClient: HttpClient) {}

  public getLinks() {
    this.httpClient
      .get<LinkResponse>('/api/link')
      .pipe(tap((r) => this.links.next(r.list)))
      .subscribe();
  }
}
