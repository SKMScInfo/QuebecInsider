import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, forkJoin, map, of, switchMap } from 'rxjs';
import { ArticleApiService } from '../article-api.service';
import { ArticleActions } from './article.actions';

@Injectable()
export class ArticleEffects {
  private readonly actions$ = inject(Actions);
  private readonly api = inject(ArticleApiService);

  loadHome$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ArticleActions.loadHome),
      switchMap(({ culture, category }) =>
        forkJoin({
          articles: this.api.getArticles(culture, category),
          trending: this.api.getTrending(culture),
          categories: this.api.getCategories(culture)
        }).pipe(
          map((result) => ArticleActions.loadHomeSuccess(result)),
          catchError((error: unknown) => of(ArticleActions.loadHomeFailure({ error: String(error) })))
        )
      )
    )
  );
}
