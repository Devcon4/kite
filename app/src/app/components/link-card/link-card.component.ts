import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { MatLegacyChipsModule as MatChipsModule } from '@angular/material/legacy-chips';
import { Link } from '../../../services/link.state';
import { ThemeState } from '../../../services/theme.state';

@Component({
  selector: 'kite-link-card',
  templateUrl: './link-card.component.html',
  styleUrls: ['./link-card.component.scss'],
  standalone: true,
  imports: [CommonModule, MatChipsModule],
})
export class LinkCardComponent implements OnInit {
  @Input()
  link: Link | undefined;
  constructor(private themeState: ThemeState) {}
  selectTagColor = this.themeState.hashColor;
  kindColor = this.themeState.ingressKindColor;

  ngOnInit(): void {}
}
