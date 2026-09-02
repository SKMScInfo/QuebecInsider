import { Component, computed, signal } from '@angular/core';
import { RouterLink, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { copy, langFromRoute } from '../../core/i18n';
import { LangCode } from '../../core/models';

@Component({
  selector: 'qi-site-header',
  standalone: true,
  imports: [RouterLink],
  template: `
    <header class="site-header">
      <a class="brand" [routerLink]="['/', lang()]">QuébecInsider <span>⚜</span></a>
      <button class="menu-button" type="button" (click)="menuOpen.set(!menuOpen())" aria-label="Menu">☰</button>
      <nav [class.open]="menuOpen()">
        @for (item of text().nav; track item; let i = $index) {
          <a [routerLink]="['/', lang(), slugs[lang()][i]]">{{ item }}</a>
        }
      </nav>
      <div class="header-actions">
        <button class="search-button" type="button" aria-label="Search">⌕</button>
        <button class="newsletter-button" type="button">Newsletter</button>
        <div class="lang-switch" aria-label="Language selector">
          <button type="button" [class.active]="lang() === 'fr'" (click)="switchLang('fr')">FR</button>
          <span>|</span>
          <button type="button" [class.active]="lang() === 'en'" (click)="switchLang('en')">EN</button>
        </div>
      </div>
    </header>
  `
})
export class SiteHeaderComponent {
  readonly menuOpen = signal(false);
  readonly lang = signal<LangCode>('fr');
  readonly text = computed(() => copy[this.lang()]);
  readonly slugs: Record<LangCode, string[]> = {
    fr: ['argent', 'bons-plans', 'bouffe', 'sorties', 'voyage', 'tech', 'vie-au-quebec'],
    en: ['money', 'deals', 'food', 'things-to-do', 'travel', 'tech', 'life-in-quebec']
  };

  constructor(private readonly router: Router) {
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe(() => {
      this.lang.set(langFromRoute(this.router.url.split('/')[1]));
      this.menuOpen.set(false);
    });
  }

  switchLang(lang: LangCode): void {
    const segments = this.router.url.split('/').filter(Boolean);
    const categoryIndex = this.slugs[this.lang()].indexOf(segments[1] ?? '');
    const translatedCategory = categoryIndex >= 0 ? this.slugs[lang][categoryIndex] : undefined;
    this.router.navigate(['/', lang, translatedCategory].filter(Boolean));
  }
}
