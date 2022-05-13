import { BehaviorSubject, fromEvent } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { Injectable } from '@angular/core';

export type ThemeType = 'dark' | 'light';
const on = 'var(--on)';
const off = 'var(--off)';

// Example of how to create StateServices.
// Store Application state as BehaviorSubjects, create functions to update state.
// In components use map to format Application State into Presentation State.
@Injectable({
    providedIn: 'root'
})
export class ThemeState {
  theme = new BehaviorSubject<ThemeType>(
    window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light'
  );

  constructor() {
    fromEvent<MediaQueryListEvent>(
      window.matchMedia('(prefers-color-scheme: dark)'),
      'change'
    )
      .pipe(
        map((e) => (e.matches ? 'dark' : 'light')),
        tap((t: ThemeType) => this.setTheme(t))
      )
      .subscribe();
  }

  setTheme(val: ThemeType) {
    this.theme.next(val);
  }

  toggleTheme() {
    const isLight = this.theme.getValue() === 'light';
    document.body.style.setProperty('--light', isLight ? off : on);
    document.body.style.setProperty('--dark', isLight ? on : off);
    this.setTheme(isLight ? 'dark' : 'light');
  }
}
