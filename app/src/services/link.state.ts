import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
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
  private httpClient = inject(HttpClient);

  links = new BehaviorSubject<Link[]>([]);

  public getLinks() {
    this.httpClient
      .get<LinkResponse>('/api/link')
      .pipe(tap((r) => this.links.next(r.list)))
      .subscribe();
  }
}
