import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Article, Category, CultureCode } from './models';

@Injectable({ providedIn: 'root' })
export class ArticleApiService {
  private readonly baseUrl = 'http://localhost:5000/api';

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
}
