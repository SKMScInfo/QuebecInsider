import { DatePipe } from '@angular/common';
import { convertToParamMap, ActivatedRoute } from '@angular/router';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { ArticleApiService } from '../../core/article-api.service';
import { Article } from '../../core/models';
import { ArticlePageComponent } from './article-page.component';

describe('ArticlePageComponent', () => {
  let fixture: ComponentFixture<ArticlePageComponent>;
  const article: Article = {
    id: 1,
    culture: 'fr-CA',
    title: "J'ai appris le français au Québec beaucoup plus vite avec ces 7 trucs",
    slug: 'apprendre-francais-quebec-7-trucs',
    description: 'Une méthode concrète.',
    body: 'Pourquoi les cours seuls ne suffisent pas',
    seoTitle: 'Article SEO',
    seoDescription: 'SEO description',
    keywords: 'français',
    insiderTip: 'Emprunte gratuitement des livres numériques.',
    categoryName: 'Vie au Québec',
    categorySlug: 'vie-au-quebec',
    imageUrl: 'https://example.com/photo.jpg',
    publishedAt: '2026-08-24T00:00:00Z',
    readingMinutes: 7,
    authorName: 'Camille Drouin',
    popularityScore: 98,
    alternateCultureSlug: 'learn-french-quebec-7-strategies'
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ArticlePageComponent],
      providers: [
        DatePipe,
        provideRouter([]),
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ lang: 'fr', slug: article.slug }))
          }
        },
        {
          provide: ArticleApiService,
          useValue: {
            getArticle: jasmine.createSpy('getArticle').and.returnValue(of(article))
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ArticlePageComponent);
    fixture.detectChanges();
  });

  it('loads and renders an article page', () => {
    const text = fixture.nativeElement.textContent as string;

    expect(text).toContain(article.title);
    expect(text).toContain(article.insiderTip);
  });
});
