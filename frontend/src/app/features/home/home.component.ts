import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { copy, langFromRoute } from '../../core/i18n';
import { Article, LangCode } from '../../core/models';
import { ArticleActions } from '../../core/state/article.actions';
import { selectArticleError, selectArticleLoading, selectArticles, selectCategories, selectTrending } from '../../core/state/article.selectors';
import { InsiderTipComponent } from '../../shared/insider-tip/insider-tip.component';
import { NewsletterComponent } from '../../shared/newsletter/newsletter.component';

@Component({
  selector: 'qi-home',
  standalone: true,
  imports: [RouterLink, DatePipe, InsiderTipComponent, NewsletterComponent],
  templateUrl: './home.component.html'
})
export class HomeComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly store = inject(Store);
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);

  readonly lang = signal<LangCode>('fr');
  readonly text = computed(() => copy[this.lang()]);
  readonly articles = this.store.selectSignal(selectArticles);
  readonly trending = this.store.selectSignal(selectTrending);
  readonly categories = this.store.selectSignal(selectCategories);
  readonly loading = this.store.selectSignal(selectArticleLoading);
  readonly error = this.store.selectSignal(selectArticleError);
  readonly dateTimezone: string | undefined = undefined;

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const lang = langFromRoute(params.get('lang'));
      this.lang.set(lang);
      const culture = copy[lang].culture;
      this.store.dispatch(ArticleActions.loadHome({ culture, category: params.get('category') ?? undefined }));
      this.title.setTitle(lang === 'fr' ? 'QuébecInsider | Bons plans et astuces au Québec' : 'QuébecInsider | Quebec deals and life hacks');
      this.meta.updateTag({ name: 'description', content: this.text().heroSubtitle });
    });
  }

  articleUrl(article: Article): string[] {
    return ['/', this.lang(), 'articles', article.slug];
  }
}
