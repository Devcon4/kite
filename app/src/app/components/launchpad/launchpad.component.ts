import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import 'css-paint-polyfill';
import { combineLatest } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  groupBy,
  keyValueSort,
  lookupSort,
} from '../../../services/arrayUtils';
import { LinkState } from '../../../services/link.state';
import { SettingState } from '../../../services/setting.state';
import { ThemeState, ThemeType } from '../../../services/theme.state';
import { LinkCardComponent } from '../link-card/link-card.component';

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
    ]
})
export class LaunchpadComponent implements OnInit {
  constructor(
    private linkState: LinkState,
    private themeState: ThemeState,
    private settingState: SettingState
  ) {}

  keySort = this.settingState.globalSettings.pipe(
    map((s) =>
      s?.groupOrder ? lookupSort(s!.groupOrder, 'Desc') : keyValueSort('Desc')
    )
  );

  groupsObs = combineLatest([this.linkState.links, this.keySort]).pipe(
    map(([l, s]) => ({ list: groupBy(l, (o) => o.group), sort: s }))
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

  ngOnInit(): void {
    this.linkState.getLinks();
    this.settingState.getSettings();
  }
}
