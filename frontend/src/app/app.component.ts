import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SiteHeaderComponent } from './shared/layout/site-header.component';
import { SiteFooterComponent } from './shared/layout/site-footer.component';

@Component({
  selector: 'qi-root',
  standalone: true,
  imports: [RouterOutlet, SiteHeaderComponent, SiteFooterComponent],
  template: `
    <qi-site-header />
    <router-outlet />
    <qi-site-footer />
  `
})
export class AppComponent {}
