import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { ArticlePageComponent } from './features/articles/article-page.component';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'fr' },
  { path: ':lang', component: HomeComponent },
  { path: ':lang/articles/:slug', component: ArticlePageComponent },
  { path: ':lang/:category', component: HomeComponent },
  { path: '**', redirectTo: 'fr' }
];
