import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { SuccessDialogService } from '../../core/services/success-dialog.service';
import { Match } from '../../core/models/match.model';
import { FAMOUS_COUNTRIES, CountryOption } from '../../core/models/country.model';
import { CountryCardComponent } from '../../shared/components/country-card/country-card.component';

@Component({
  selector: 'app-matches',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    CountryCardComponent
  ],
  template: `
    <div class="matches-container">
      <div class="page-header">
        <div>
          <h1 class="page-title">Match Management</h1>
          <p class="page-subtitle">Start new matches, update live scores, and finish matches</p>
        </div>
      </div>

      <!-- Start Match Form Card -->
      <mat-card class="start-match-card">
        <mat-card-header>
          <mat-card-title class="card-title">
            <mat-icon class="title-icon">add_circle_outline</mat-icon> Start a New World Cup Match
          </mat-card-title>
        </mat-card-header>
        <mat-card-content>
          <form [formGroup]="startForm" (ngSubmit)="onStartMatch()" class="start-form">
            <div class="teams-selection-row">
              <!-- Left / Home Country -->
              <mat-form-field appearance="outline" class="team-field">
                <mat-label>Home Country (Left)</mat-label>
                <mat-select formControlName="homeTeam" placeholder="Select Home Country">
                  <mat-option *ngFor="let country of countries" [value]="country.name">
                    <div class="country-option">
                      <img [src]="country.flagUrl" [alt]="country.name + ' flag'" class="option-flag" />
                      <span>{{ country.name }}</span>
                    </div>
                  </mat-option>
                </mat-select>
              </mat-form-field>

              <div class="vs-divider">
                <span>VS</span>
              </div>

              <!-- Right / Away Country -->
              <mat-form-field appearance="outline" class="team-field">
                <mat-label>Away Country (Right)</mat-label>
                <mat-select formControlName="awayTeam" placeholder="Select Away Country">
                  <mat-option *ngFor="let country of countries" [value]="country.name">
                    <div class="country-option">
                      <img [src]="country.flagUrl" [alt]="country.name + ' flag'" class="option-flag" />
                      <span>{{ country.name }}</span>
                    </div>
                  </mat-option>
                </mat-select>
              </mat-form-field>
            </div>

            <div class="extra-fields-row">
              <mat-form-field appearance="outline" class="half-field">
                <mat-label>Stadium / Location</mat-label>
                <input matInput formControlName="location" placeholder="e.g. Estadio Azteca" />
              </mat-form-field>

              <mat-form-field appearance="outline" class="half-field">
                <mat-label>Scheduled Time</mat-label>
                <input matInput type="datetime-local" formControlName="scheduledAt" />
              </mat-form-field>
            </div>

            <div class="form-actions">
              <button
                mat-flat-button
                color="primary"
                type="submit"
                class="start-btn"
                [disabled]="startForm.invalid || startingMatch">
                <mat-spinner *ngIf="startingMatch" diameter="20" class="btn-spinner"></mat-spinner>
                <span *ngIf="!startingMatch">
                  <mat-icon>play_arrow</mat-icon> Start Match
                </span>
              </button>
            </div>
          </form>
        </mat-card-content>
      </mat-card>

      <!-- Active Matches Score Updates & Finish -->
      <div class="active-matches-section">
        <h2 class="section-title">Active In-Progress Matches</h2>

        <div *ngIf="loadingActive" class="spinner-container">
          <mat-spinner diameter="36"></mat-spinner>
        </div>

        <div *ngIf="!loadingActive && activeMatches.length === 0" class="no-active-box">
          <mat-icon>info</mat-icon>
          <span>No active matches at the moment. Use the form above to start one!</span>
        </div>

        <div *ngIf="!loadingActive && activeMatches.length > 0" class="active-list">
          <div *ngFor="let match of activeMatches" class="active-match-card">
            <div class="match-card-top">
              <div class="match-info">
                <span class="match-tag">Match #{{ match.id }}</span>
                <span *ngIf="match.location" class="location-tag">📍 {{ match.location }}</span>
              </div>
              <span class="live-chip">LIVE</span>
            </div>

            <!-- Side by side Cards with VS -->
            <div class="match-display-row">
              <app-country-card [countryName]="match.homeTeam.name" [score]="match.homeTeam.score" side="home"></app-country-card>
              <div class="active-vs">VS</div>
              <app-country-card [countryName]="match.awayTeam.name" [score]="match.awayTeam.score" side="away"></app-country-card>
            </div>

            <!-- Controls: Update Score & Finish Match -->
            <div class="match-controls-bar">
              <div class="score-input-group">
                <div class="score-input-item">
                  <label [attr.for]="'home-score-' + match.id">{{ match.homeTeam.name }} Score:</label>
                  <input
                    [id]="'home-score-' + match.id"
                    type="number"
                    min="0"
                    [(ngModel)]="scoreMap[match.id].homeScore"
                    class="score-number-input" />
                </div>
                <div class="score-input-item">
                  <label [attr.for]="'away-score-' + match.id">{{ match.awayTeam.name }} Score:</label>
                  <input
                    [id]="'away-score-' + match.id"
                    type="number"
                    min="0"
                    [(ngModel)]="scoreMap[match.id].awayScore"
                    class="score-number-input" />
                </div>
                <button
                  mat-flat-button
                  color="accent"
                  class="update-score-btn"
                  (click)="onUpdateScore(match.id)">
                  <mat-icon>edit</mat-icon> Update Score
                </button>
              </div>

              <div class="finish-group">
                <button
                  mat-stroked-button
                  color="warn"
                  class="finish-match-btn"
                  (click)="onFinishMatch(match.id)">
                  <mat-icon>flag</mat-icon> Finish Match
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .matches-container {
      max-width: 960px;
      margin: 0 auto;
    }
    .page-header {
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
    .start-match-card {
      background: #ffffff;
      border-radius: 16px;
      border: 1px solid #e2e8f0;
      box-shadow: 0 4px 16px rgba(10, 54, 157, 0.06);
      margin-bottom: 32px;
      padding: 16px;
    }
    .card-title {
      display: flex;
      align-items: center;
      gap: 8px;
      color: #0a369d;
      font-size: 1.25rem;
      font-weight: 600;
    }
    .title-icon {
      color: #0a369d;
    }
    .start-form {
      margin-top: 16px;
    }
    .teams-selection-row {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .team-field {
      flex: 1;
    }
    .country-option {
      display: flex;
      align-items: center;
      gap: 10px;
    }
    .option-flag {
      width: 24px;
      height: 16px;
      object-fit: cover;
      border-radius: 2px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.2);
    }
    .vs-divider {
      font-weight: 800;
      color: #0a369d;
      background: #e0e7ff;
      width: 38px;
      height: 38px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.85rem;
      flex-shrink: 0;
    }
    .extra-fields-row {
      display: flex;
      gap: 16px;
    }
    .half-field {
      flex: 1;
    }
    .form-actions {
      display: flex;
      justify-content: flex-end;
      margin-top: 8px;
    }
    .start-btn {
      background-color: #0a369d !important;
      color: #ffffff !important;
      padding: 8px 24px;
      border-radius: 8px;
      font-weight: 600;
    }
    .section-title {
      font-size: 1.3rem;
      font-weight: 700;
      color: #0f172a;
      margin-bottom: 16px;
    }
    .spinner-container {
      display: flex;
      justify-content: center;
      padding: 32px 0;
    }
    .no-active-box {
      display: flex;
      align-items: center;
      gap: 12px;
      background: #f1f5f9;
      color: #475569;
      padding: 16px 20px;
      border-radius: 12px;
      font-size: 0.95rem;
    }
    .active-match-card {
      background: #ffffff;
      border: 1px solid #e2e8f0;
      border-radius: 16px;
      padding: 20px;
      margin-bottom: 20px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.03);
    }
    .match-card-top {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 14px;
    }
    .match-info {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .match-tag {
      font-weight: 700;
      color: #0a369d;
      font-size: 0.9rem;
    }
    .location-tag {
      font-size: 0.85rem;
      color: #64748b;
    }
    .live-chip {
      background: #dbeafe;
      color: #1d4ed8;
      font-weight: 700;
      font-size: 0.75rem;
      padding: 2px 10px;
      border-radius: 12px;
      border: 1px solid #93c5fd;
    }
    .match-display-row {
      display: flex;
      align-items: center;
      gap: 16px;
      margin-bottom: 20px;
    }
    .active-vs {
      font-weight: 800;
      color: #ffffff;
      background: linear-gradient(135deg, #0a369d 0%, #2563eb 100%);
      width: 40px;
      height: 40px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 0.9rem;
      flex-shrink: 0;
      box-shadow: 0 4px 8px rgba(37, 99, 235, 0.3);
    }
    .match-controls-bar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding-top: 16px;
      border-top: 1px solid #f1f5f9;
      flex-wrap: wrap;
      gap: 16px;
    }
    .score-input-group {
      display: flex;
      align-items: center;
      gap: 16px;
      flex-wrap: wrap;
    }
    .score-input-item {
      display: flex;
      align-items: center;
      gap: 8px;
      label {
        font-size: 0.9rem;
        font-weight: 600;
        color: #334155;
      }
    }
    .score-number-input {
      width: 60px;
      padding: 8px 10px;
      border: 1px solid #cbd5e1;
      border-radius: 6px;
      font-size: 1rem;
      font-weight: 700;
      color: #0a369d;
      text-align: center;
    }
    .update-score-btn {
      background-color: #2563eb !important;
      color: #ffffff !important;
      border-radius: 6px;
      font-weight: 600;
    }
    .finish-match-btn {
      border-color: #ef4444;
      color: #ef4444;
      border-radius: 6px;
      font-weight: 600;
    }
  `]
})
export class MatchesComponent implements OnInit {
  countries: CountryOption[] = FAMOUS_COUNTRIES;
  startForm!: FormGroup;
  activeMatches: Match[] = [];
  scoreMap: { [matchId: number]: { homeScore: number; awayScore: number } } = {};
  loadingActive: boolean = false;
  startingMatch: boolean = false;

  constructor(
    private fb: FormBuilder,
    private scoreboardService: ScoreboardService,
    private successDialogService: SuccessDialogService
  ) {}

  ngOnInit(): void {
    this.startForm = this.fb.group({
      homeTeam: ['Mexico', Validators.required],
      awayTeam: ['Canada', Validators.required],
      location: ['', Validators.required],
      scheduledAt: [this.nowISOForInput()]
    });

    this.loadActiveMatches();
  }

  // Datetime-local input value, computed fresh each time so a new match's scheduledAt never
  // silently reuses a previous submission's timestamp — combined with a required, blank
  // location default, this avoids colliding with the backend's "same location + same time"
  // in-progress uniqueness rule when starting several matches back-to-back.
  private nowISOForInput(): string {
    return new Date().toISOString().slice(0, 16);
  }

  loadActiveMatches(): void {
    this.loadingActive = true;
    this.scoreboardService.getSummary().subscribe({
      next: (data) => {
        this.activeMatches = data;
        this.scoreMap = {};
        for (const m of data) {
          this.scoreMap[m.id] = {
            homeScore: m.homeTeam.score,
            awayScore: m.awayTeam.score
          };
        }
        this.loadingActive = false;
      },
      error: () => {
        this.loadingActive = false;
      }
    });
  }

  onStartMatch(): void {
    if (this.startForm.invalid) return;

    this.startingMatch = true;
    const val = this.startForm.value;
    this.scoreboardService.startMatch({
      homeTeam: val.homeTeam,
      awayTeam: val.awayTeam,
      location: val.location,
      scheduledAt: val.scheduledAt ? new Date(val.scheduledAt).toISOString() : undefined
    }).subscribe({
      next: () => {
        this.startingMatch = false;
        this.successDialogService.openSuccess('Match started successfully.', 'Match Started');
        this.startForm.patchValue({ location: '', scheduledAt: this.nowISOForInput() });
        this.loadActiveMatches();
      },
      error: () => {
        this.startingMatch = false;
      }
    });
  }

  onUpdateScore(matchId: number): void {
    const scores = this.scoreMap[matchId];
    if (!scores) return;

    this.scoreboardService.updateScore(matchId, {
      homeScore: Number(scores.homeScore),
      awayScore: Number(scores.awayScore)
    }).subscribe({
      next: () => {
        this.successDialogService.openSuccess('Score updated successfully.', 'Score Updated');
        this.loadActiveMatches();
      }
    });
  }

  onFinishMatch(matchId: number): void {
    this.scoreboardService.finishMatch(matchId).subscribe({
      next: () => {
        this.successDialogService.openSuccess('Match finished successfully.', 'Match Finished');
        this.loadActiveMatches();
      }
    });
  }
}
