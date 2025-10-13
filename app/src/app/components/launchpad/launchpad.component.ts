import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit,
  afterNextRender,
  computed,
  inject,
  signal,
} from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
// import 'css-paint-polyfill'; // Breaks vitest and doesn't actually polyfill in firefox.
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { combineLatest } from 'rxjs';
import { debounceTime, map, startWith, tap } from 'rxjs/operators';
import {
  groupBy,
  keyValueSort,
  lookupSort,
} from '../../../services/arrayUtils';
import { Link, LinkState } from '../../../services/link.state';
import { SettingState } from '../../../services/setting.state';
import { ThemeState, ThemeType } from '../../../services/theme.state';
import { LinkCardComponent } from '../link-card/link-card.component';

import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import Fuse from 'fuse.js';
const filterLinks = (links: Link[], filter: string) => {
  if (!filter || filter.length < 2) return links;
  const fuse = new Fuse(links, {
    keys: [
      { name: 'name', weight: 3.0 },
      { name: 'tags', weight: 2.0 },
      { name: 'path', weight: 1.5 },
      { name: 'description', weight: 1.0 },
      { name: 'group', weight: 0.5 },
      { name: 'kind', weight: 0.5 },
    ],
    threshold: 0.3,
    includeScore: true,
    isCaseSensitive: false,
    useExtendedSearch: true,
    shouldSort: true,
    minMatchCharLength: 2,
  });

  return fuse.search(filter).map((r) => r.item);
};

@Component({
  selector: 'kite-launchpad',
  templateUrl: './launchpad.component.html',
  styleUrls: ['./launchpad.component.scss'],
  imports: [
    LinkCardComponent,
    CommonModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    ReactiveFormsModule,
    RouterLink,
  ],
})
export class LaunchpadComponent implements OnInit {
  constructor() {
    afterNextRender(() => {
      setTimeout(() => {
        this.initialLoadComplete.set(true);
      }, 500);
    });
  }

  private linkState = inject(LinkState);
  private themeState = inject(ThemeState);
  private settingState = inject(SettingState);
  private activatedRoute = inject(ActivatedRoute);
  private router = inject(Router);

  textFilter = new FormControl('', { nonNullable: true });

  keySort = this.settingState.globalSettings.pipe(
    map((s) =>
      s?.groupOrder ? lookupSort(s!.groupOrder, 'Desc') : keyValueSort('Desc')
    )
  );
  initialLoadComplete = signal(false);
  enterAnimation = computed(() =>
    this.initialLoadComplete() ? 'card-enter' : ''
  );

  groups = toSignal(
    combineLatest([
      this.linkState.linksFiltered,
      this.keySort,
      this.textFilter.valueChanges.pipe(
        debounceTime(120),
        startWith(this.activatedRoute.snapshot.queryParamMap.get('q') || '')
      ),
    ]).pipe(
      tap(([_, __, t]) =>
        this.router.navigate([], {
          queryParams: t ? { q: t } : {},
          preserveFragment: !t,
        })
      ),
      map(([l, s, t]) => [filterLinks(l, t), s] as const),
      map(([l, s]) => ({ list: groupBy(l, (o) => o.group), sort: s }))
    )
  );

  isDark = this.themeState.theme.pipe(
    map<ThemeType, boolean>((l) => l === 'dark')
  );
  isLight = this.themeState.theme.pipe(
    map<ThemeType, boolean>((l) => l === 'light')
  );

  toggleTheme = () => this.themeState.toggleTheme();
  borderColor = this.themeState.ingressKindColor;

  appName = this.settingState.globalSettings.pipe(map((s) => s?.appName));

  clearTextFilter() {
    this.textFilter.setValue('');
  }

  ngOnInit(): void {
    this.linkState.getLinks();
    this.settingState.getSettings();

    this.textFilter.setValue(
      this.activatedRoute.snapshot.queryParamMap.get('q') || ''
    );
  }
}
