import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { Subject, switchMap, tap } from 'rxjs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatPaginatorModule } from '@angular/material/paginator';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { Match } from '../../core/models/match.model';
import { MatchRowComponent } from '../../shared/components/match-row/match-row.component';

@Component({
  selector: 'app-history',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatButtonModule, MatIconModule, MatPaginatorModule, MatchRowComponent],
  template: `
    <div class="history-container">
      <div class="page-header">
        <div>
          <h1 class="page-title">Match History</h1>
          <p class="page-subtitle">Browsing all historical and current matches (10 entries per page, most recent activity first)</p>
        </div>
        <button mat-stroked-button color="primary" (click)="loadHistory(currentPage)" [disabled]="loading" class="refresh-btn">
          <mat-icon [class.spinning]="loading">refresh</mat-icon> Refresh
        </button>
      </div>

      <div *ngIf="loading" class="spinner-container">
        <mat-spinner diameter="40"></mat-spinner>
      </div>

      <div *ngIf="!loading && matches.length === 0" class="empty-state">
        <mat-icon class="empty-icon">history</mat-icon>
        <h3>No matches found in history</h3>
        <p>Matches created in the <strong>Matches</strong> tab will appear here.</p>
      </div>

      <div *ngIf="!loading && matches.length > 0" class="matches-list">
        <app-match-row
          *ngFor="let match of matches"
          [match]="match">
        </app-match-row>

        <div class="pagination-bar">
          <button
            mat-flat-button
            class="nav-page-btn"
            [disabled]="currentPage <= 1 || loading"
            (click)="goToPage(currentPage - 1)">
            <mat-icon>chevron_left</mat-icon> Previous Page
          </button>
          <span class="page-indicator">Page {{ currentPage }}</span>
          <button
            mat-flat-button
            class="nav-page-btn"
            [disabled]="matches.length < 10 || loading"
            (click)="goToPage(currentPage + 1)">
            Next Page <mat-icon>chevron_right</mat-icon>
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .history-container {
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
    .pagination-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 8px;
      margin-top: 16px;
      background: #ffffff;
      border-radius: 12px;
      border: 1px solid #e2e8f0;
    }
    .page-indicator {
      font-weight: 600;
      color: #0a369d;
    }
    .nav-page-btn {
      background-color: #0a369d !important;
      color: #ffffff !important;
      border-radius: 8px;
    }
    .nav-page-btn:disabled {
      background-color: #e2e8f0 !important;
      color: #94a3b8 !important;
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
export class HistoryComponent implements OnInit {
  matches: Match[] = [];
  currentPage: number = 1;
  loading: boolean = false;

  // Page changes go through this trigger + switchMap (rather than a plain .subscribe() per
  // call) so that rapidly clicking Next/Previous — or navigating away before a page finishes
  // loading — cancels the previous in-flight request instead of letting a stale, out-of-order
  // response overwrite a newer page's data. takeUntilDestroyed also cancels anything still in
  // flight when the component is destroyed (e.g. the user switches to Summary mid-request).
  private pageRequest$ = new Subject<number>();

  constructor(private scoreboardService: ScoreboardService, private destroyRef: DestroyRef) {}

  ngOnInit(): void {
    this.pageRequest$
      .pipe(
        tap(() => (this.loading = true)),
        switchMap((page) => this.scoreboardService.getHistory(page).pipe(tap(() => (this.currentPage = page)))),
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

    this.loadHistory(this.currentPage);
  }

  loadHistory(page: number): void {
    this.pageRequest$.next(page);
  }

  goToPage(page: number): void {
    if (page >= 1) {
      this.loadHistory(page);
    }
  }
}
