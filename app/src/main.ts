import { provideHttpClient } from '@angular/common/http';
import { enableProdMode, importProvidersFrom } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app/app.component';
import { environment } from './environments/environment';

const routes: Routes = [
  {
    path: 'launchpad',
    loadComponent: () =>
      import('./app/components/launchpad/launchpad.component').then(
        (c) => c.LaunchpadComponent
      ),
  },
  {
    pathMatch: 'full',
    redirectTo: 'launchpad',
    path: '**',
  },
  {
    pathMatch: 'full',
    redirectTo: 'launchpad',
    path: '',
  },
];

if (environment.production) {
  enableProdMode();
}

bootstrapApplication(AppComponent, {
  providers: [
    importProvidersFrom(
      RouterModule.forRoot(routes, {
        anchorScrolling: 'enabled',
        // scrollPositionRestoration: 'enabled',
      })
    ),
    provideHttpClient(),
  ],
}).catch((err) => console.error(err));
