
import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
    selector: 'kite-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    imports: [RouterModule]
})
export class AppComponent {
  private httpClient = inject(HttpClient);


  ngOnInit() {
    this.httpClient
      .get('/background-paint.js', { responseType: 'blob' as 'json' })
      .subscribe((r: any) => {
        const fleckBlob = new Blob([r], { type: 'text/javascript' });
        const fleckUrl = URL.createObjectURL(fleckBlob);
        (CSS as any).paintWorklet.addModule(fleckUrl);
      });
  }
}
