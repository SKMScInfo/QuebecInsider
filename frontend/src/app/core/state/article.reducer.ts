import { createReducer, on } from '@ngrx/store';
import { Article, Category } from '../models';
import { ArticleActions } from './article.actions';

export interface ArticleState {
  articles: Article[];
  trending: Article[];
  categories: Category[];
  loading: boolean;
  error?: string;
}

export const initialArticleState: ArticleState = {
  articles: [],
  trending: [],
  categories: [],
  loading: false
};

export const articleReducer = createReducer(
  initialArticleState,
  on(ArticleActions.loadHome, (state) => ({ ...state, loading: true, error: undefined })),
  on(ArticleActions.loadHomeSuccess, (state, { articles, trending, categories }) => ({
    ...state,
    articles,
    trending,
    categories,
    loading: false
  })),
  on(ArticleActions.loadHomeFailure, (state, { error }) => ({ ...state, loading: false, error })),
  on(ArticleActions.clearError, (state) => ({ ...state, error: undefined }))
);
