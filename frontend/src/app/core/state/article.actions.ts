import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Article, Category, CultureCode } from '../models';

export const ArticleActions = createActionGroup({
  source: 'Articles',
  events: {
    'Load Home': props<{ culture: CultureCode; category?: string }>(),
    'Load Home Success': props<{ articles: Article[]; trending: Article[]; categories: Category[] }>(),
    'Load Home Failure': props<{ error: string }>(),
    'Clear Error': emptyProps()
  }
});
