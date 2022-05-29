import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, tap } from 'rxjs';

export type GlobalSetting = {
  appName: string;
  groupOrder: { [k: string]: number };
};

type SettingResponse = {
  settings: GlobalSetting;
};

@Injectable({
  providedIn: 'root',
})
export class SettingState {
  globalSettings = new BehaviorSubject<GlobalSetting | undefined>(undefined);

  constructor(private httpClient: HttpClient) {}

  public getSettings() {
    this.httpClient
      .get<SettingResponse>('/api/setting')
      .pipe(
        tap((r) => this.globalSettings.next(r.settings)),
        tap((r) => console.log(r))
      )
      .subscribe();
  }
}
