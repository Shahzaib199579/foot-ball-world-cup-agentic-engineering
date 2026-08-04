import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { ScoreboardService } from './scoreboard.service';

describe('ScoreboardService', () => {
  let service: ScoreboardService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5000/matches';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()]
    });
    service = TestBed.inject(ScoreboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('getSummary() calls GET /matches/summary', () => {
    service.getSummary().subscribe();
    const req = httpMock.expectOne(`${apiUrl}/summary`);
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getHistory(page) calls GET /matches/history?page={page}', () => {
    service.getHistory(2).subscribe();
    const req = httpMock.expectOne((r) => r.url === `${apiUrl}/history` && r.params.get('page') === '2');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('getMatch(id) calls GET /matches/{id}', () => {
    service.getMatch(1).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/1`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('startMatch(request) calls POST /matches', () => {
    const request = { homeTeam: 'Mexico', awayTeam: 'Canada', location: 'Estadio Azteca', scheduledAt: '2026-08-04T00:00:00Z' };
    service.startMatch(request).subscribe();
    const req = httpMock.expectOne(apiUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(request);
    req.flush({});
  });

  it('updateScore(id, request) calls PUT /matches/{id}/score', () => {
    const request = { homeScore: 2, awayScore: 1 };
    service.updateScore(1, request).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/1/score`);
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(request);
    req.flush({});
  });

  it('finishMatch(id) calls POST /matches/{id}/finish', () => {
    service.finishMatch(1).subscribe();
    const req = httpMock.expectOne(`${apiUrl}/1/finish`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });
});
