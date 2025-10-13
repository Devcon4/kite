import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class FilterState {
  selectedTags = new BehaviorSubject<string[]>([]);

  private cleanTag(tag: string) {
    return tag.trim().toLowerCase();
  }

  public selectTag(tag: string) {
    this.selectedTags.next([
      ...this.selectedTags.getValue(),
      this.cleanTag(tag),
    ]);
  }

  public deselectTag(tag: string) {
    this.selectedTags.next(
      this.selectedTags.getValue().filter((t) => t !== this.cleanTag(tag))
    );
  }

  public clearTags() {
    this.selectedTags.next([]);
  }

  public toggleTag(tag: string) {
    if (this.selectedTags.getValue().includes(this.cleanTag(tag))) {
      this.deselectTag(tag);
      return;
    }

    this.selectTag(tag);
  }
}
