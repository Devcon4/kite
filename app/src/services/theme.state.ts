import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export type ThemeType = 'dark' | 'light';
const on = 'var(--on)';
const off = 'var(--off)';

export const colorLookup = [
  '--clr1',
  '--clr2',
  '--clr3',
  '--clr4',
  '--clr5',
  '--clr6',
];

// Example of how to create StateServices.
// Store Application state as BehaviorSubjects, create functions to update state.
// In components use map to format Application State into Presentation State.
@Injectable({
  providedIn: 'root',
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

  hashColor(tag: string) {
    return `var(${colorLookup[(tag.charCodeAt(0) % colorLookup.length) - 1]})`;
  }

  ingressKindColor(kind?: string) {
    const kindLookup = {
      Static: 0,
      Ingress: 2,
      HttpRoute: 1,
      IngressRoute: 3,
    };
    return `var(${
      colorLookup[kindLookup[kind as keyof typeof kindLookup] || 0]
    })`;
  }
}
