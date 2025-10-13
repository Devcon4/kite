import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, combineLatest, map, tap } from 'rxjs';
import { FilterState } from './filter.state';

export type Link = {
  group: string;
  path: string;
  name: string;
  key: string;
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
  private filterState = inject(FilterState);

  links = new BehaviorSubject<Link[]>([]);

  linksFiltered = combineLatest([
    this.links,
    this.filterState.selectedTags,
  ]).pipe(
    tap(([links, selectedTags]) =>
      console.log('Filtering links', { links, selectedTags })
    ),
    map(([links, selectedTags]) => {
      if (selectedTags.length === 0) {
        return links;
      }
      // Filter links that have at least one of the selected tags
      return links.filter((link) =>
        link.tags.some((tag) => selectedTags.includes(tag.trim().toLowerCase()))
      );
    })
  );

  public getLinks() {
    this.httpClient
      .get<LinkResponse>('/api/link')
      .pipe(
        map((r) =>
          r.list.map((l) => ({
            ...l,
            key: l.name.toLowerCase() + l.path.toLowerCase(),
          }))
        ),
        tap((r) => this.links.next(r))
      )
      .subscribe();
  }
}
