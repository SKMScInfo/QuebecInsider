import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Article, Category, CultureCode } from './models';

export interface CreateArticleTranslationPayload {
  title: string;
  slug: string;
  description: string;
  body: string;
  seoTitle: string;
  seoDescription: string;
  keywords: string;
  insiderTip: string;
}

export interface CreateArticlePayload {
  categoryKey: string;
  imageUrl: string;
  authorName: string;
  readingMinutes: number;
  initialPopularityScore: number;
  french: CreateArticleTranslationPayload;
  english: CreateArticleTranslationPayload;
}

@Injectable({ providedIn: 'root' })
export class ArticleApiService {
  private readonly baseUrl = environment.apiUrl;

  constructor(private readonly http: HttpClient) {}

  getArticles(culture: CultureCode, category?: string): Observable<Article[]> {
    const params = category ? new HttpParams().set('category', category) : undefined;
    return this.http.get<Article[]>(`${this.baseUrl}/${culture}/articles`, { params });
  }

  getTrending(culture: CultureCode): Observable<Article[]> {
    return this.http.get<Article[]>(`${this.baseUrl}/${culture}/articles/trending`);
  }

  getArticle(culture: CultureCode, slug: string): Observable<Article> {
    return this.http.get<Article>(`${this.baseUrl}/${culture}/articles/${slug}`);
  }

  getCategories(culture: CultureCode): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/${culture}/categories`);
  }

  search(culture: CultureCode, query: string): Observable<Article[]> {
    const params = new HttpParams().set('q', query);
    return this.http.get<Article[]>(`${this.baseUrl}/${culture}/search`, { params });
  }

  loginAdmin(username: string, password: string): Observable<{ accessToken: string; expiresIn: number }> {
    return this.http.post<{ accessToken: string; expiresIn: number }>(`${this.baseUrl}/auth/token`, {
      username,
      password
    });
  }

  createArticle(token: string, payload: CreateArticlePayload): Observable<{ id: number; frenchUrl: string; englishUrl: string }> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<{ id: number; frenchUrl: string; englishUrl: string }>(
      `${this.baseUrl}/admin/articles`,
      payload,
      { headers }
    );
  }

  evictCache(token: string): Observable<{ message: string }> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.post<{ message: string }>(`${this.baseUrl}/admin/cache/evict`, {}, { headers });
  }
}