import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatChipsModule } from '@angular/material/chips';
import { Router } from '@angular/router';
import { FilterState } from '../../../services/filter.state';
import { Link } from '../../../services/link.state';
import { ThemeState } from '../../../services/theme.state';
@Component({
  selector: 'kite-link-card',
  templateUrl: './link-card.component.html',
  styleUrls: ['./link-card.component.scss'],
  imports: [CommonModule, MatChipsModule],
})
export class LinkCardComponent implements OnInit {
  private themeState = inject(ThemeState);
  private filterState = inject(FilterState);
  private router = inject(Router);

  readonly link = input<Link>();
  selectTagColor = this.themeState.hashColor;
  kindColor = this.themeState.ingressKindColor;
  // lookup for if a tag is selecte
  tagLookup = toSignal(this.filterState.selectedTags);

  tags = computed(() => {
    const link = this.link();
    const lookup = this.tagLookup();
    if (!link || !lookup) return [];

    if (lookup.length > 0) {
      this.router.navigate([], {
        queryParamsHandling: 'preserve',
        preserveFragment: false,
      });
    }
    return link.tags.map((tag) => ({
      value: tag,
      selected: lookup.includes(tag),
    }));
  });

  ngOnInit(): void {}

  toggleTagFilter(tag: string) {
    this.filterState.toggleTag(tag);
  }
}
