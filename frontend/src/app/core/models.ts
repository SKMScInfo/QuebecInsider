export type LangCode = 'fr' | 'en';
export type CultureCode = 'fr-CA' | 'en-CA';

export interface Article {
  id: number;
  culture: CultureCode;
  title: string;
  slug: string;
  description: string;
  body: string;
  seoTitle: string;
  seoDescription: string;
  keywords: string;
  insiderTip: string;
  categoryName: string;
  categorySlug: string;
  imageUrl: string;
  publishedAt: string;
  readingMinutes: number;
  authorName: string;
  popularityScore: number;
  alternateCultureSlug?: string;
}

export interface Category {
  key: string;
  name: string;
  slug: string;
  description: string;
}
