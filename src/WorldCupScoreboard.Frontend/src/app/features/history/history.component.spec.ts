import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { HistoryComponent } from './history.component';
import { Match } from '../../core/models/match.model';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { MatchStatus } from '../../core/models/match.model';

function makeMatches(count: number, status: MatchStatus) {
  return Array.from({ length: count }, (_, i) => ({
    id: i + 1,
    homeTeam: { name: 'Home' + i, score: 0 },
    awayTeam: { name: 'Away' + i, score: 0 },
    status
  }));
}

describe('HistoryComponent', () => {
  let scoreboardServiceSpy: jasmine.SpyObj<ScoreboardService>;

  function createFixture() {
    TestBed.configureTestingModule({
      imports: [HistoryComponent],
      providers: [{ provide: ScoreboardService, useValue: scoreboardServiceSpy }]
    });
    return TestBed.createComponent(HistoryComponent);
  }

  it('calls getHistory(1) on init and renders a status badge per match', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getHistory']);
    scoreboardServiceSpy.getHistory.and.returnValue(of(makeMatches(3, MatchStatus.Finished)));

    const fixture = createFixture();
    fixture.detectChanges();

    expect(scoreboardServiceSpy.getHistory).toHaveBeenCalledWith(1);
    expect(fixture.nativeElement.querySelectorAll('.status-badge').length).toBe(3);
  });

  it('shows pagination controls with the current page indicator', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getHistory']);
    scoreboardServiceSpy.getHistory.and.returnValue(of(makeMatches(10, MatchStatus.InProgress)));

    const fixture = createFixture();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.page-indicator').textContent).toContain('Page 1');
  });

  it('disables "Previous Page" on page 1 and enables "Next Page" when a full page (10) was returned', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getHistory']);
    scoreboardServiceSpy.getHistory.and.returnValue(of(makeMatches(10, MatchStatus.InProgress)));

    const fixture = createFixture();
    fixture.detectChanges();

    const buttons: HTMLButtonElement[] = fixture.nativeElement.querySelectorAll('.nav-page-btn');
    expect(buttons[0].disabled).toBeTrue();
    expect(buttons[1].disabled).toBeFalse();
  });

  it('does not throw when rendering a page containing a finished match', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getHistory']);
    scoreboardServiceSpy.getHistory.and.returnValue(of(makeMatches(1, MatchStatus.Finished)));

    const fixture = createFixture();

    expect(() => fixture.detectChanges()).not.toThrow();
  });

  it('cancels a stale page request when a newer page is requested before it resolves (rapid pagination, spec.md Edge Case 3)', () => {
    const page1$ = new Subject<Match[]>();
    const page2$ = new Subject<Match[]>();
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getHistory']);
    scoreboardServiceSpy.getHistory.and.returnValues(page1$, page2$);

    const fixture = createFixture();
    fixture.detectChanges(); // ngOnInit -> loadHistory(1), subscribes to page1$

    fixture.componentInstance.goToPage(2); // switchMap cancels page1$, subscribes to page2$

    page2$.next(makeMatches(2, MatchStatus.InProgress));
    page1$.next(makeMatches(10, MatchStatus.InProgress)); // stale page 1 response, must be ignored

    expect(fixture.componentInstance.currentPage).toBe(2);
    expect(fixture.componentInstance.matches.length).toBe(2);
  });
});
