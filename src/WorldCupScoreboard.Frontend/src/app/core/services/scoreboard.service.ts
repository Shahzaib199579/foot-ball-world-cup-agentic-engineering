import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Match, StartMatchRequest, UpdateScoreRequest } from '../models/match.model';

@Injectable({
  providedIn: 'root'
})
export class ScoreboardService {
  // Use relative or configurable API base URL, defaulting to local API server port 5000
  private apiUrl = 'http://localhost:5000/matches';

  constructor(private http: HttpClient) {}

  setApiUrl(baseUrl: string): void {
    this.apiUrl = baseUrl;
  }

  getSummary(): Observable<Match[]> {
    return this.http.get<Match[]>(`${this.apiUrl}/summary`);
  }

  getHistory(page: number = 1): Observable<Match[]> {
    const params = new HttpParams().set('page', page.toString());
    return this.http.get<Match[]>(`${this.apiUrl}/history`, { params });
  }

  getMatch(id: number): Observable<Match> {
    return this.http.get<Match>(`${this.apiUrl}/${id}`);
  }

  startMatch(request: StartMatchRequest): Observable<Match> {
    return this.http.post<Match>(this.apiUrl, request);
  }

  updateScore(id: number, request: UpdateScoreRequest): Observable<Match> {
    return this.http.put<Match>(`${this.apiUrl}/${id}/score`, request);
  }

  finishMatch(id: number): Observable<Match> {
    return this.http.post<Match>(`${this.apiUrl}/${id}/finish`, {});
  }
}
