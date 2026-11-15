import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ArticleApiService, CreateArticlePayload } from '../../core/article-api.service';

@Component({
  selector: 'qi-admin-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <main class="admin-shell">
      <header class="admin-header">
        <div>
          <span class="badge">Administration JWT</span>
          <h1>Studio de publication QuebecInsider</h1>
        </div>
        @if (token()) {
          <div class="header-actions">
            <button type="button" class="btn-secondary" (click)="purgeCache()">Purger le cache Redis</button>
            <button type="button" class="btn-outline" (click)="logout()">Déconnexion</button>
          </div>
        }
      </header>

      @if (statusMessage()) {
        <div class="alert" [class.error]="isError()">
          {{ statusMessage() }}
          @if (createdLinks(); as links) {
            <div class="created-links">
              <a [routerLink]="links.frenchUrl">Voir l'article FR →</a>
              <a [routerLink]="links.englishUrl">Voir l'article EN →</a>
            </div>
          }
        </div>
      }

      @if (!token()) {
        <section class="card login-card">
          <h2>Connexion Administrateur</h2>
          <p>Authentifiez-vous pour obtenir un jeton JWT valide 2 heures.</p>
          <form (ngSubmit)="login()" class="form-grid">
            <label>
              Utilisateur
              <input type="text" [(ngModel)]="username" name="username" required />
            </label>
            <label>
              Mot de passe
              <input type="password" [(ngModel)]="password" name="password" required />
            </label>
            <button type="submit" class="btn-primary" [disabled]="loading()">
              {{ loading() ? 'Connexion...' : 'Se connecter (JWT)' }}
            </button>
          </form>
        </section>
      } @else {
        <form (ngSubmit)="publishArticle()" class="admin-form">
          <section class="card">
            <h2>1. Métadonnées générales</h2>
            <div class="grid-3">
              <label>
                Catégorie
                <select [(ngModel)]="form.categoryKey" name="categoryKey" required>
                  <option value="money">Argent / Money</option>
                  <option value="deals">Bons plans / Deals</option>
                  <option value="food">Bouffe / Food</option>
                  <option value="going-out">Sorties / Things To Do</option>
                  <option value="travel">Voyage / Travel</option>
                  <option value="tech">Tech</option>
                  <option value="life">Vie au Québec / Life In Quebec</option>
                </select>
              </label>

              <label>
                Auteur(e)
                <input type="text" [(ngModel)]="form.authorName" name="authorName" required />
              </label>

              <label>
                Temps de lecture (min)
                <input type="number" [(ngModel)]="form.readingMinutes" name="readingMinutes" min="1" max="60" required />
              </label>
            </div>

            <div class="grid-2">
              <label>
                URL de l'image principale (Unsplash / CDN)
                <input type="url" [(ngModel)]="form.imageUrl" name="imageUrl" required />
              </label>

              <label>
                Score de popularité initial
                <input type="number" [(ngModel)]="form.initialPopularityScore" name="initialPopularityScore" min="1" max="1000" required />
              </label>
            </div>
          </section>

          <div class="bilingual-grid">
            <!-- COLONNE FRANÇAIS -->
            <section class="card">
              <h2>🇫🇷 Version Française (fr-CA)</h2>
              <label>
                Titre (FR)
                <input type="text" [(ngModel)]="form.french.title" (ngModelChange)="autoSlugFr($event)" name="frTitle" required />
              </label>
              <label>
                Slug URL (FR)
                <input type="text" [(ngModel)]="form.french.slug" name="frSlug" required />
              </label>
              <label>
                Résumé court (Description)
                <textarea rows="2" [(ngModel)]="form.french.description" name="frDesc" required></textarea>
              </label>
              <label>
                Contenu de l'article (HTML : &lt;p&gt;, &lt;h2&gt;, &lt;ul&gt;...)
                <textarea rows="8" [(ngModel)]="form.french.body" name="frBody" required></textarea>
              </label>
              <label>
                Astuce d'initié (Insider Tip)
                <input type="text" [(ngModel)]="form.french.insiderTip" name="frTip" required />
              </label>
              <label>
                Mots-clés SEO (séparés par des virgules)
                <input type="text" [(ngModel)]="form.french.keywords" name="frKeywords" required />
              </label>
            </section>

            <!-- COLONNE ANGLAIS -->
            <section class="card">
              <h2>🇨🇦 English Version (en-CA)</h2>
              <label>
                Title (EN)
                <input type="text" [(ngModel)]="form.english.title" (ngModelChange)="autoSlugEn($event)" name="enTitle" required />
              </label>
              <label>
                URL Slug (EN)
                <input type="text" [(ngModel)]="form.english.slug" name="enSlug" required />
              </label>
              <label>
                Short Summary (Description)
                <textarea rows="2" [(ngModel)]="form.english.description" name="enDesc" required></textarea>
              </label>
              <label>
                Article Body (HTML: &lt;p&gt;, &lt;h2&gt;, &lt;ul&gt;...)
                <textarea rows="8" [(ngModel)]="form.english.body" name="enBody" required></textarea>
              </label>
              <label>
                Insider Tip
                <input type="text" [(ngModel)]="form.english.insiderTip" name="enTip" required />
              </label>
              <label>
                SEO Keywords (comma-separated)
                <input type="text" [(ngModel)]="form.english.keywords" name="enKeywords" required />
              </label>
            </section>
          </div>

          <div class="submit-bar">
            <button type="submit" class="btn-primary btn-large" [disabled]="loading()">
              {{ loading() ? 'Publication en cours...' : '🚀 Publier l’article bilingue et rafraîchir Redis' }}
            </button>
          </div>
        </form>
      }
    </main>
  `,
  styles: [`
    .admin-shell { max-width: 1200px; margin: 2rem auto; padding: 0 1.25rem 4rem; }
    .admin-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; flex-wrap: wrap; gap: 1rem; }
    .badge { display: inline-block; background: #1e3a8a; color: #fff; font-size: 0.75rem; font-weight: 700; padding: 0.25rem 0.6rem; border-radius: 999px; text-transform: uppercase; }
    h1 { margin: 0.35rem 0 0; font-size: 1.85rem; }
    .header-actions { display: flex; gap: 0.75rem; }
    .card { background: #fff; border: 1px solid #e2e8f0; border-radius: 14px; padding: 1.5rem; margin-bottom: 1.5rem; box-shadow: 0 4px 12px rgba(15, 23, 42, 0.04); }
    .login-card { max-width: 440px; margin: 3rem auto; }
    .form-grid, .card { display: flex; flex-direction: column; gap: 1rem; }
    .grid-3 { display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 1rem; }
    .grid-2 { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1rem; }
    .bilingual-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(460px, 1fr)); gap: 1.5rem; }
    label { display: flex; flex-direction: column; gap: 0.35rem; font-weight: 600; font-size: 0.9rem; color: #1e293b; }
    input, select, textarea { padding: 0.65rem 0.8rem; border: 1px solid #cbd5e1; border-radius: 8px; font-size: 0.95rem; font-family: inherit; }
    textarea { resize: vertical; }
    .btn-primary { background: #0f172a; color: #fff; border: none; padding: 0.75rem 1.25rem; border-radius: 8px; font-weight: 700; cursor: pointer; }
    .btn-secondary { background: #2563eb; color: #fff; border: none; padding: 0.6rem 1rem; border-radius: 8px; font-weight: 600; cursor: pointer; }
    .btn-outline { background: transparent; border: 1px solid #cbd5e1; padding: 0.6rem 1rem; border-radius: 8px; font-weight: 600; cursor: pointer; }
    .btn-large { width: 100%; font-size: 1.05rem; padding: 1rem; }
    .alert { padding: 1rem 1.25rem; border-radius: 10px; background: #ecfdf5; border: 1px solid #10b981; color: #065f46; margin-bottom: 1.5rem; font-weight: 600; }
    .alert.error { background: #fef2f2; border-color: #ef4444; color: #991b1b; }
    .created-links { margin-top: 0.5rem; display: flex; gap: 1.25rem; }
    .created-links a { color: #047857; text-decoration: underline; }
  `]
})
export class AdminPageComponent {
  username = 'admin';
  password = '';
  token = signal<string | null>(null);
  loading = signal(false);
  statusMessage = signal<string | null>(null);
  isError = signal(false);
  createdLinks = signal<{ frenchUrl: string; englishUrl: string } | null>(null);

  form: CreateArticlePayload = {
    categoryKey: 'deals',
    imageUrl: 'https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&w=1200&q=80',
    authorName: 'Kushal Sangwan',
    readingMinutes: 5,
    initialPopularityScore: 95,
    french: {
      title: '',
      slug: '',
      description: '',
      body: '<p class="lead">Introduction de l’article...</p>\n<h2>1. Premier point</h2>\n<p>Détails ici...</p>',
      seoTitle: '',
      seoDescription: '',
      keywords: 'Québec, astuces, économies',
      insiderTip: ''
    },
    english: {
      title: '',
      slug: '',
      description: '',
      body: '<p class="lead">Article introduction...</p>\n<h2>1. First takeaway</h2>\n<p>Details here...</p>',
      seoTitle: '',
      seoDescription: '',
      keywords: 'Quebec, tips, savings',
      insiderTip: ''
    }
  };

  constructor(private readonly api: ArticleApiService) {}

  login(): void {
    this.loading.set(true);
    this.statusMessage.set(null);
    this.api.loginAdmin(this.username, this.password).subscribe({
      next: (res) => {
        this.token.set(res.accessToken);
        this.loading.set(false);
        this.isError.set(false);
        this.statusMessage.set('Authentification JWT réussie ! Vous pouvez maintenant publier un article.');
      },
      error: () => {
        this.loading.set(false);
        this.isError.set(true);
        this.statusMessage.set('Identifiants invalides (401 Unauthorized).');
      }
    });
  }

  logout(): void {
    this.token.set(null);
    this.statusMessage.set(null);
  }

  autoSlugFr(title: string): void {
    this.form.french.slug = this.slugify(title);
    this.form.french.seoTitle = title;
  }

  autoSlugEn(title: string): void {
    this.form.english.slug = this.slugify(title);
    this.form.english.seoTitle = title;
  }

  publishArticle(): void {
    const jwt = this.token();
    if (!jwt) return;

    this.form.french.seoDescription = this.form.french.description;
    this.form.english.seoDescription = this.form.english.description;

    this.loading.set(true);
    this.createdLinks.set(null);

    this.api.createArticle(jwt, this.form).subscribe({
      next: (res) => {
        this.loading.set(false);
        this.isError.set(false);
        this.statusMessage.set(`Article #${res.id} publié avec succès dans PostgreSQL et cache Redis purgé !`);
        this.createdLinks.set({ frenchUrl: res.frenchUrl, englishUrl: res.englishUrl });
      },
      error: (err) => {
        this.loading.set(false);
        this.isError.set(true);
        this.statusMessage.set(err?.error?.error || 'Erreur lors de la publication de l’article.');
      }
    });
  }

  purgeCache(): void {
    const jwt = this.token();
    if (!jwt) return;

    this.api.evictCache(jwt).subscribe({
      next: (res) => {
        this.isError.set(false);
        this.statusMessage.set(res.message);
      },
      error: () => {
        this.isError.set(true);
        this.statusMessage.set('Impossible de purger le cache Redis.');
      }
    });
  }

  private slugify(text: string): string {
    return text
      .toLowerCase()
      .normalize('NFD')
      .replace(/[\u0300-\u036f]/g, '')
      .replace(/[^a-z0-9]+/g, '-')
      .replace(/(^-|-$)/g, '');
  }
}