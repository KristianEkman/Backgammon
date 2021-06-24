export class Language {
  constructor(public code: string, public name: string) {}
  static get List(): Language[] {
    return [
      new Language('en', 'English'),
      new Language('zh', '中國人'),
      new Language('es', 'Español'),
      new Language('ar', 'عربى'),
      new Language('fr', 'français'),
      new Language('sv', 'Svenska'),
      new Language('el', 'Ελληνικά')
      // new Language('x', 'X')
    ];
  }
}
