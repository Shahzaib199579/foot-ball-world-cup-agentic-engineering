import { Routes } from '@angular/router';
import { SummaryComponent } from './features/summary/summary.component';
import { HistoryComponent } from './features/history/history.component';
import { MatchesComponent } from './features/matches/matches.component';

export const routes: Routes = [
  { path: '', redirectTo: 'summary', pathMatch: 'full' },
  { path: 'summary', component: SummaryComponent },
  { path: 'history', component: HistoryComponent },
  { path: 'matches', component: MatchesComponent },
  { path: '**', redirectTo: 'summary' }
];
