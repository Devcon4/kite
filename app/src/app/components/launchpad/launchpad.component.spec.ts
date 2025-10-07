import { ComponentFixture, TestBed } from '@angular/core/testing';
import { beforeAll, beforeEach, describe, expect, it } from 'vitest';

import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { LaunchpadComponent } from './launchpad.component';

// Mock window.matchMedia for jsdom (required by ThemeState)
beforeAll(() => {
  Object.defineProperty(window, 'matchMedia', {
    writable: true,
    value: (query: string) => ({
      matches: false,
      media: query,
      onchange: null,
      addListener: () => {}, // deprecated
      removeListener: () => {}, // deprecated
      addEventListener: () => {},
      removeEventListener: () => {},
      dispatchEvent: () => true,
    }),
  });
});

describe('LaunchpadComponent', () => {
  let component: LaunchpadComponent;
  let fixture: ComponentFixture<LaunchpadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LaunchpadComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(LaunchpadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    // SKIPPED: css-paint-polyfill (imported in launchpad.component.ts) causes stack overflow in jsdom
    // The polyfill executes at module load time and breaks jsdom's CSS implementation
    // Attempts to mock with vi.mock() fail because Angular CLI bundles the polyfill before Vitest can intercept it
    // This component requires testing in a real browser environment or e2e tests
    expect(component).toBeTruthy();
  });
});
