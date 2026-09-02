import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { copy } from '../../core/i18n';
import { LangCode } from '../../core/models';

@Component({
  selector: 'qi-newsletter',
  standalone: true,
  imports: [FormsModule],
  template: `
    <section class="newsletter">
      <div>
        <span class="eyebrow">QuébecInsider</span>
        <h2>{{ text.newsletterTitle }}</h2>
        <p>{{ text.newsletterBody }}</p>
      </div>
      <form>
        <input type="email" name="email" [(ngModel)]="email" placeholder="email@example.com" />
        <button type="submit">{{ text.newsletterCta }}</button>
      </form>
    </section>
  `
})
export class NewsletterComponent {
  @Input({ required: true }) lang: LangCode = 'fr';
  email = '';

  get text() {
    return copy[this.lang];
  }
}
