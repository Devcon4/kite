import { ComponentFixture, TestBed } from '@angular/core/testing';
import { beforeAll, beforeEach, describe, expect, it } from 'vitest';

import { LinkCardComponent } from './link-card.component';

// Mock window.matchMedia for jsdom
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

describe('LinkCardComponent', () => {
  let component: LinkCardComponent;
  let fixture: ComponentFixture<LinkCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LinkCardComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(LinkCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
