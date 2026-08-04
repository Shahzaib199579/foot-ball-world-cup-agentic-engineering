import { TestBed } from '@angular/core/testing';
import { MatchRowComponent } from './match-row.component';
import { MatchStatus } from '../../../core/models/match.model';

function makeMatch(status: MatchStatus) {
  return {
    id: 1,
    homeTeam: { name: 'Uruguay', score: 6 },
    awayTeam: { name: 'Italy', score: 6 },
    status,
    location: 'Estadio Azteca',
    scheduledAt: '2026-08-04T00:00:00Z'
  };
}

describe('MatchRowComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [MatchRowComponent]
    });
  });

  it('renders a Home card, a VS badge, then an Away card, in that order', () => {
    const fixture = TestBed.createComponent(MatchRowComponent);
    fixture.componentInstance.match = makeMatch(MatchStatus.InProgress);
    fixture.detectChanges();

    const cards = fixture.nativeElement.querySelectorAll('app-country-card');
    expect(cards.length).toBe(2);
    expect(cards[0].querySelector('.country-card').classList).toContain('home');
    expect(cards[1].querySelector('.country-card').classList).toContain('away');
    expect(fixture.nativeElement.querySelector('.vs-badge')).toBeTruthy();
  });

  it('shows a LIVE badge for an in-progress match', () => {
    const fixture = TestBed.createComponent(MatchRowComponent);
    fixture.componentInstance.match = makeMatch(MatchStatus.InProgress);
    fixture.detectChanges();

    const badge = fixture.nativeElement.querySelector('.status-badge');
    expect(badge.textContent).toContain('LIVE');
    expect(badge.classList).toContain('live');
  });

  it('shows a FINISHED badge for a finished match, without throwing', () => {
    const fixture = TestBed.createComponent(MatchRowComponent);
    fixture.componentInstance.match = makeMatch(MatchStatus.Finished);

    expect(() => fixture.detectChanges()).not.toThrow();

    const badge = fixture.nativeElement.querySelector('.status-badge');
    expect(badge.textContent).toContain('FINISHED');
    expect(badge.classList).toContain('finished');
  });
});
