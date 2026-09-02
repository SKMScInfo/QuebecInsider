import { CultureCode, LangCode } from './models';

export const copy = {
  fr: {
    culture: 'fr-CA' as CultureCode,
    nav: ['Argent', 'Bons plans', 'Bouffe', 'Sorties', 'Voyage', 'Tech', 'Vie au Québec'],
    heroTitle: 'Les bons plans du Québec que vous auriez aimé connaître plus tôt.',
    heroSubtitle: 'Économisez, découvrez et simplifiez votre quotidien au Québec.',
    primaryCta: 'Découvrir les astuces',
    secondaryCta: 'Voir les bons plans',
    missed: 'À ne pas manquer',
    categories: 'Explorer par sujet',
    trending: 'Les plus populaires',
    newsletterTitle: 'Recevez les meilleures trouvailles du Québec',
    newsletterBody: 'Une infolettre concise avec rabais, services utiles et astuces locales.',
    newsletterCta: "S'inscrire",
    search: 'Rechercher',
    read: 'Lire',
    minutes: 'min de lecture',
    tipTitle: "L'astuce Insider"
  },
  en: {
    culture: 'en-CA' as CultureCode,
    nav: ['Money', 'Deals', 'Food', 'Things To Do', 'Travel', 'Tech', 'Life In Quebec'],
    heroTitle: 'Quebec life hacks you will wish you knew sooner.',
    heroSubtitle: 'Save money, discover smarter options and simplify everyday life in Quebec.',
    primaryCta: 'Discover Tips',
    secondaryCta: 'See Deals',
    missed: "Don't Miss",
    categories: 'Explore By Topic',
    trending: 'Most Popular',
    newsletterTitle: 'Get Quebec’s best finds in your inbox',
    newsletterBody: 'A concise newsletter with deals, useful services and local tips.',
    newsletterCta: 'Sign Up',
    search: 'Search',
    read: 'Read',
    minutes: 'min read',
    tipTitle: 'Insider Tip'
  }
};

export function langFromRoute(value: string | null): LangCode {
  return value === 'en' ? 'en' : 'fr';
}
