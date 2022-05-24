import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import 'css-paint-polyfill';
import { map } from 'rxjs/operators';
import { groupBy, keyValueSort } from '../../../services/arrayUtils';
import { LinkState } from '../../../services/link.state';
import { ThemeState, ThemeType } from '../../../services/theme.state';
import { LinkCardComponent } from '../link-card/link-card.component';

@Component({
  selector: 'kite-launchpad',
  templateUrl: './launchpad.component.html',
  styleUrls: ['./launchpad.component.scss'],
  standalone: true,
  imports: [
    LinkCardComponent,
    CommonModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class LaunchpadComponent implements OnInit {
  constructor(private linkState: LinkState, private themeState: ThemeState) {}

  keySort = keyValueSort('Desc');

  links = this.linkState.links.pipe(map((l) => groupBy(l, (o) => o.group)));

  isDark = this.themeState.theme.pipe(
    map<ThemeType, boolean>((l) => l === 'dark')
  );
  isLight = this.themeState.theme.pipe(
    map<ThemeType, boolean>((l) => l === 'light')
  );

  toggleTheme = () => this.themeState.toggleTheme();
  borderColor = this.themeState.ingressKindColor;

  ngOnInit(): void {
    this.linkState.getLinks();
  }
}
