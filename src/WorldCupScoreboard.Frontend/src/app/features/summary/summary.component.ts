import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Subject, switchMap, tap } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { Match } from '../../core/models/match.model';
import { MatchRowComponent } from '../../shared/components/match-row/match-row.component';

@Component({
  selector: 'app-summary',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatButtonModule, MatIconModule, MatchRowComponent],
  template: `
    <div class="summary-container">
      <div class="page-header">
        <div>
          <h1 class="page-title">Live Match Summary</h1>
          <p class="page-subtitle">In-progress matches ordered by total score descending (most recent first on ties)</p>
        </div>
        <button mat-stroked-button color="primary" (click)="loadSummary()" [disabled]="loading" class="refresh-btn">
          <mat-icon [class.spinning]="loading">refresh</mat-icon> Refresh
        </button>
      </div>

      <div *ngIf="loading" class="spinner-container">
        <mat-spinner diameter="40"></mat-spinner>
      </div>

      <div *ngIf="!loading && matches.length === 0" class="empty-state">
        <mat-icon class="empty-icon">sports_soccer</mat-icon>
        <h3>No matches currently in progress</h3>
        <p>Go to the <strong>Matches</strong> tab to start a new World Cup match.</p>
      </div>

      <div *ngIf="!loading && matches.length > 0" class="matches-list">
        <app-match-row
          *ngFor="let match of matches"
          [match]="match">
        </app-match-row>
      </div>
    </div>
  `,
  styles: [`
    .summary-container {
      max-width: 960px;
      margin: 0 auto;
    }
    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 24px;
      padding-bottom: 16px;
      border-bottom: 1px solid #e2e8f0;
    }
    .page-title {
      font-size: 1.75rem;
      font-weight: 700;
      color: #003366;
      margin: 0 0 4px 0;
    }
    .page-subtitle {
      font-size: 0.9rem;
      color: #64748b;
      margin: 0;
    }
    .refresh-btn {
      border-color: #0a369d;
      color: #0a369d;
      border-radius: 8px;
    }
    .spinner-container {
      display: flex;
      justify-content: center;
      padding: 48px 0;
    }
    .empty-state {
      text-align: center;
      padding: 64px 24px;
      background: #f8fafc;
      border: 2px dashed #cbd5e1;
      border-radius: 16px;
      color: #64748b;
    }
    .empty-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      color: #94a3b8;
      margin-bottom: 12px;
    }
    .empty-state h3 {
      font-size: 1.2rem;
      font-weight: 600;
      color: #334155;
      margin: 0 0 8px 0;
    }
    .empty-state p {
      margin: 0;
      font-size: 0.95rem;
    }
    .spinning {
      animation: spin 1s linear infinite;
    }
    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }
  `]
})
export class SummaryComponent implements OnInit {
  matches: Match[] = [];
  loading: boolean = false;

  // Refreshes go through this trigger + switchMap (rather than a plain .subscribe() per call)
  // so that rapidly re-triggering a refresh — or navigating away before a request resolves —
  // cancels the previous in-flight request instead of letting a stale, out-of-order response
  // overwrite newer state. takeUntilDestroyed also cancels anything still in flight when the
  // component is destroyed (e.g. the user switches tabs before the response arrives).
  private refresh$ = new Subject<void>();

  constructor(private scoreboardService: ScoreboardService, private destroyRef: DestroyRef) {}

  ngOnInit(): void {
    this.refresh$
      .pipe(
        tap(() => (this.loading = true)),
        switchMap(() => this.scoreboardService.getSummary()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: (data) => {
          this.matches = data;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
        }
      });

    this.loadSummary();
  }

  loadSummary(): void {
    this.refresh$.next();
  }
}
