import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { Match, MatchStatus } from '../../../core/models/match.model';
import { CountryCardComponent } from '../country-card/country-card.component';

@Component({
  selector: 'app-match-row',
  standalone: true,
  imports: [CommonModule, MatChipsModule, MatIconModule, CountryCardComponent],
  template: `
    <div class="match-row-wrapper" [attr.data-match-id]="match.id">
      <div class="match-meta-bar" *ngIf="showMeta">
        <span class="match-id">Match #{{ match.id }}</span>
        <div class="meta-right">
          <span *ngIf="match.location" class="meta-item">
            <mat-icon class="meta-icon">location_on</mat-icon> {{ match.location }}
          </span>
          <span *ngIf="match.scheduledAt" class="meta-item">
            <mat-icon class="meta-icon">schedule</mat-icon> {{ match.scheduledAt | date:'short' }}
          </span>
          <span class="status-badge" [ngClass]="isFinished ? 'finished' : 'live'">
            {{ isFinished ? 'FINISHED' : 'LIVE' }}
          </span>
        </div>
      </div>

      <div class="cards-row">
        <app-country-card
          [countryName]="match.homeTeam.name"
          [score]="match.homeTeam.score"
          side="home">
        </app-country-card>

        <div class="vs-badge">
          <span class="vs-text">VS</span>
        </div>

        <app-country-card
          [countryName]="match.awayTeam.name"
          [score]="match.awayTeam.score"
          side="away">
        </app-country-card>
      </div>
    </div>
  `,
  styles: [`
    .match-row-wrapper {
      margin-bottom: 20px;
      padding: 18px 24px;
      background: #f8fafc;
      border-radius: 16px;
      border: 1px solid #e2e8f0;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.03);
    }
    .match-meta-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 12px;
      font-size: 0.85rem;
      color: #64748b;
    }
    .match-id {
      font-weight: 600;
      color: #334155;
    }
    .meta-right {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .meta-item {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    .meta-icon {
      font-size: 16px;
      width: 16px;
      height: 16px;
      color: #94a3b8;
    }
    .status-badge {
      font-size: 0.75rem;
      font-weight: 700;
      padding: 2px 10px;
      border-radius: 12px;
      letter-spacing: 0.05em;
    }
    .status-badge.live, .status-badge.inprogress {
      background: #dbeafe;
      color: #1d4ed8;
      border: 1px solid #93c5fd;
    }
    .status-badge.finished {
      background: #f1f5f9;
      color: #64748b;
      border: 1px solid #cbd5e1;
    }
    .cards-row {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .vs-badge {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 44px;
      height: 44px;
      border-radius: 50%;
      background: linear-gradient(135deg, #0a369d 0%, #2563eb 100%);
      color: #ffffff;
      font-weight: 800;
      font-size: 0.95rem;
      box-shadow: 0 4px 10px rgba(37, 99, 235, 0.3);
      flex-shrink: 0;
      letter-spacing: 0.05em;
    }
    @media (max-width: 640px) {
      .cards-row {
        flex-direction: column;
      }
      .vs-badge {
        margin: 4px 0;
      }
    }
  `]
})
export class MatchRowComponent {
  @Input() match!: Match;
  @Input() showMeta: boolean = true;

  get isFinished(): boolean {
    return this.match.status === MatchStatus.Finished;
  }
}
