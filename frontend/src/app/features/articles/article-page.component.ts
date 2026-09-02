import { DatePipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ArticleApiService } from '../../core/article-api.service';
import { copy, langFromRoute } from '../../core/i18n';
import { Article, LangCode } from '../../core/models';
import { InsiderTipComponent } from '../../shared/insider-tip/insider-tip.component';

@Component({
  selector: 'qi-article-page',
  standalone: true,
  imports: [DatePipe, RouterLink, InsiderTipComponent],
  template: `
    @if (article(); as article) {
      <main class="article-page">
        <a class="back-link" [routerLink]="['/', lang()]">← QuébecInsider</a>
        <article>
          <header>
            <span class="category">{{ article.categoryName }}</span>
            <h1>{{ article.title }}</h1>
            <p>{{ article.description }}</p>
            <small>{{ article.publishedAt | date:'longDate':dateTimezone:text.culture }} · {{ article.readingMinutes }} {{ text.minutes }} · {{ article.authorName }}</small>
          </header>
          <img [src]="article.imageUrl" [alt]="article.title" />
          <div class="article-content" [innerHTML]="article.body"></div>
          <qi-insider-tip
            [title]="text.tipTitle"
            [tip]="article.insiderTip || (lang() === 'fr'
              ? 'Vérifiez toujours les versions municipales et provinciales d’un service: les avantages se cumulent parfois.'
              : 'Always check both municipal and provincial versions of a service: benefits sometimes stack.')"
          />
        </article>
      </main>
    }
  `
})
export class ArticlePageComponent implements OnInit {
  readonly lang = signal<LangCode>('fr');
  readonly article = signal<Article | null>(null);
  readonly dateTimezone: string | undefined = undefined;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly api: ArticleApiService,
    private readonly title: Title,
    private readonly meta: Meta
  ) {}

  get text() {
    return copy[this.lang()];
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      const lang = langFromRoute(params.get('lang'));
      this.lang.set(lang);
      this.api.getArticle(copy[lang].culture, params.get('slug') ?? '').subscribe((article) => {
        this.article.set(article);
        this.title.setTitle(article.seoTitle);
        this.meta.updateTag({ name: 'description', content: article.seoDescription });
        this.meta.updateTag({ property: 'og:title', content: article.title });
        this.meta.updateTag({ property: 'og:description', content: article.description });
      });
    });
  }
}
