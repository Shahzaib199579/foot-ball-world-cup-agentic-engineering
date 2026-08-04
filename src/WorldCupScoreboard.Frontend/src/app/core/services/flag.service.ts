import { Injectable } from '@angular/core';
import { FAMOUS_COUNTRIES } from '../models/country.model';

const FALLBACK_FLAG_URL =
  'data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="40" height="28" viewBox="0 0 40 28"><rect width="40" height="28" fill="%230a369d"/><text x="50%25" y="60%25" dominant-baseline="middle" text-anchor="middle" fill="white" font-size="14" font-weight="bold">⚽</text></svg>';

@Injectable({
  providedIn: 'root'
})
export class FlagService {
  getFlagUrl(countryName: string): string {
    const match = FAMOUS_COUNTRIES.find(
      (c) => c.name.toLowerCase() === countryName.trim().toLowerCase()
    );
    return match ? match.flagUrl : FALLBACK_FLAG_URL;
  }
}
