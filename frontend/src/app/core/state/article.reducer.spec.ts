import { ArticleActions } from './article.actions';
import { articleReducer, initialArticleState } from './article.reducer';

describe('articleReducer', () => {
  it('sets loading when home content starts loading', () => {
    const state = articleReducer(initialArticleState, ArticleActions.loadHome({ culture: 'fr-CA' }));

    expect(state.loading).toBeTrue();
    expect(state.error).toBeUndefined();
  });

  it('stores a visible API error when home loading fails', () => {
    const state = articleReducer(initialArticleState, ArticleActions.loadHomeFailure({ error: 'API unavailable' }));

    expect(state.loading).toBeFalse();
    expect(state.error).toContain('API unavailable');
  });
});
