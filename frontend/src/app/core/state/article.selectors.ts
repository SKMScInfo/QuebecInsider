import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ArticleState } from './article.reducer';

export const selectArticleState = createFeatureSelector<ArticleState>('articles');
export const selectArticles = createSelector(selectArticleState, (state) => state.articles);
export const selectTrending = createSelector(selectArticleState, (state) => state.trending);
export const selectCategories = createSelector(selectArticleState, (state) => state.categories);
export const selectArticleLoading = createSelector(selectArticleState, (state) => state.loading);
export const selectArticleError = createSelector(selectArticleState, (state) => state.error);
