import { ApplicationConfig, LOCALE_ID, importProvidersFrom } from '@angular/core';
import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideRouter, withInMemoryScrolling } from '@angular/router';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { routes } from './app.routes';
import { articleReducer } from './core/state/article.reducer';
import { ArticleEffects } from './core/state/article.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withInMemoryScrolling({ scrollPositionRestoration: 'enabled' })),
    provideHttpClient(withFetch()),
    provideClientHydration(withEventReplay()),
    provideStore({ articles: articleReducer }),
    provideEffects([ArticleEffects]),
    { provide: LOCALE_ID, useValue: 'fr-CA' }
  ]
};
