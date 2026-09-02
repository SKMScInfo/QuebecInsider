import { Component } from '@angular/core';

@Component({
  selector: 'qi-site-footer',
  standalone: true,
  template: `
    <footer class="site-footer">
      <div>
        <strong>QuébecInsider ⚜</strong>
        <p>Insider knowledge for everyday life in Quebec.</p>
      </div>
      <a href="/robots.txt">Robots</a>
      <a href="/sitemap.xml">Sitemap</a>
    </footer>
  `
})
export class SiteFooterComponent {}
