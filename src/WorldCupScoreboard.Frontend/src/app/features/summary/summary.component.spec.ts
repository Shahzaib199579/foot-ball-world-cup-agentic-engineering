import { TestBed } from '@angular/core/testing';
import { of, Subject } from 'rxjs';
import { SummaryComponent } from './summary.component';
import { Match } from '../../core/models/match.model';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { MatchStatus } from '../../core/models/match.model';

function makeMatch(id: number) {
  return {
    id,
    homeTeam: { name: 'Mexico', score: 0 },
    awayTeam: { name: 'Canada', score: 5 },
    status: MatchStatus.InProgress
  };
}

describe('SummaryComponent', () => {
  let scoreboardServiceSpy: jasmine.SpyObj<ScoreboardService>;

  function createFixture() {
    TestBed.configureTestingModule({
      imports: [SummaryComponent],
      providers: [{ provide: ScoreboardService, useValue: scoreboardServiceSpy }]
    });
    return TestBed.createComponent(SummaryComponent);
  }

  it('calls getSummary() on init and renders one MatchRowComponent per match', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getSummary']);
    scoreboardServiceSpy.getSummary.and.returnValue(of([makeMatch(1), makeMatch(2)]));

    const fixture = createFixture();
    fixture.detectChanges();

    expect(scoreboardServiceSpy.getSummary).toHaveBeenCalled();
    expect(fixture.nativeElement.querySelectorAll('app-match-row').length).toBe(2);
  });

  it('shows an empty state when there are no in-progress matches', () => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getSummary']);
    scoreboardServiceSpy.getSummary.and.returnValue(of([]));

    const fixture = createFixture();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('app-match-row').length).toBe(0);
    expect(fixture.nativeElement.querySelector('.empty-state')).toBeTruthy();
  });

  it('cancels a stale in-flight request when refreshed again before it resolves (rapid tab switching, spec.md Edge Case 3)', () => {
    const first$ = new Subject<Match[]>();
    const second$ = new Subject<Match[]>();
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', ['getSummary']);
    scoreboardServiceSpy.getSummary.and.returnValues(first$, second$);

    const fixture = createFixture();
    fixture.detectChanges(); // ngOnInit -> first loadSummary() call, subscribes to first$

    fixture.componentInstance.loadSummary(); // second call -> switchMap cancels first$, subscribes to second$

    second$.next([makeMatch(2)]);
    first$.next([makeMatch(1)]); // stale response, arrives after the newer one — must be ignored

    expect(fixture.componentInstance.matches.length).toBe(1);
    expect(fixture.componentInstance.matches[0].id).toBe(2);
  });
});
