import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, inject } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { Link } from '../../../services/link.state';
import { ThemeState } from '../../../services/theme.state';

@Component({
    selector: 'kite-link-card',
    templateUrl: './link-card.component.html',
    styleUrls: ['./link-card.component.scss'],
    imports: [CommonModule, MatChipsModule]
})
export class LinkCardComponent implements OnInit {
  private themeState = inject(ThemeState);

  @Input()
  link: Link | undefined;
  selectTagColor = this.themeState.hashColor;
  kindColor = this.themeState.ingressKindColor;

  ngOnInit(): void {}
}
