import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { FlagService } from '../../../core/services/flag.service';

@Component({
  selector: 'app-country-card',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  template: `
    <mat-card class="country-card" [ngClass]="side">
      <div class="card-content">
        <div class="flag-container">
          <img [src]="flagUrl" [alt]="countryName + ' flag'" class="country-flag" />
        </div>
        <span class="country-name">{{ countryName }}</span>
        <span class="score-badge">{{ score }}</span>
      </div>
    </mat-card>
  `,
  styles: [`
    .country-card {
      flex: 1;
      background: #ffffff;
      border: 1px solid #e2e8f0;
      border-radius: 12px;
      box-shadow: 0 4px 12px rgba(10, 54, 157, 0.06);
      transition: transform 0.2s ease, box-shadow 0.2s ease;
      padding: 14px 20px;
    }
    .country-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 18px rgba(10, 54, 157, 0.12);
    }
    .card-content {
      display: flex;
      align-items: center;
      gap: 14px;
    }
    .flag-container {
      width: 42px;
      height: 28px;
      display: flex;
      align-items: center;
      justify-content: center;
      border-radius: 4px;
      overflow: hidden;
      box-shadow: 0 2px 4px rgba(0,0,0,0.15);
      background: #f8fafc;
      flex-shrink: 0;
    }
    .country-flag {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
    .country-name {
      font-size: 1.15rem;
      font-weight: 600;
      color: #0f172a;
      flex-grow: 1;
      letter-spacing: -0.01em;
    }
    .score-badge {
      font-size: 1.5rem;
      font-weight: 700;
      color: #0a369d;
      background: #eef2ff;
      padding: 4px 14px;
      border-radius: 8px;
      min-width: 32px;
      text-align: center;
    }
    .home {
      border-left: 4px solid #0a369d;
    }
    .away {
      border-right: 4px solid #2563eb;
    }
  `]
})
export class CountryCardComponent {
  @Input() countryName: string = '';
  @Input() score: number = 0;
  @Input() side: 'home' | 'away' = 'home';

  constructor(private flagService: FlagService) {}

  get flagUrl(): string {
    return this.flagService.getFlagUrl(this.countryName);
  }
}
