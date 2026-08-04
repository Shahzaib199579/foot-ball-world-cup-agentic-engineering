import { TestBed } from '@angular/core/testing';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { of } from 'rxjs';
import { MatchesComponent } from './matches.component';
import { ScoreboardService } from '../../core/services/scoreboard.service';
import { SuccessDialogService } from '../../core/services/success-dialog.service';
import { MatchStatus } from '../../core/models/match.model';

function makeActiveMatch(id: number) {
  return {
    id,
    homeTeam: { name: 'Uruguay', score: 6 },
    awayTeam: { name: 'Italy', score: 6 },
    status: MatchStatus.InProgress
  };
}

describe('MatchesComponent', () => {
  let scoreboardServiceSpy: jasmine.SpyObj<ScoreboardService>;
  let successDialogSpy: jasmine.SpyObj<SuccessDialogService>;

  function createFixture() {
    TestBed.configureTestingModule({
      imports: [MatchesComponent],
      providers: [
        provideNoopAnimations(),
        { provide: ScoreboardService, useValue: scoreboardServiceSpy },
        { provide: SuccessDialogService, useValue: successDialogSpy }
      ]
    });
    return TestBed.createComponent(MatchesComponent);
  }

  beforeEach(() => {
    scoreboardServiceSpy = jasmine.createSpyObj('ScoreboardService', [
      'getSummary',
      'startMatch',
      'updateScore',
      'finishMatch'
    ]);
    successDialogSpy = jasmine.createSpyObj('SuccessDialogService', ['openSuccess']);
    scoreboardServiceSpy.getSummary.and.returnValue(of([]));
  });

  it('populates the Home/Away dropdowns from the bundled country list', () => {
    const fixture = createFixture();
    fixture.detectChanges();

    expect(fixture.componentInstance.countries.length).toBeGreaterThan(0);
  });

  it('calls startMatch(...) on submit and opens the success dialog on success', () => {
    scoreboardServiceSpy.startMatch.and.returnValue(of(makeActiveMatch(1)));
    const fixture = createFixture();
    fixture.detectChanges();

    fixture.componentInstance.startForm.patchValue({
      homeTeam: 'Mexico',
      awayTeam: 'Canada',
      location: 'Estadio Azteca'
    });
    fixture.componentInstance.onStartMatch();

    expect(scoreboardServiceSpy.startMatch).toHaveBeenCalled();
    expect(successDialogSpy.openSuccess).toHaveBeenCalledWith(
      'Match started successfully.',
      'Match Started'
    );
  });

  it('calls updateScore(...) and opens the success dialog on success', () => {
    scoreboardServiceSpy.getSummary.and.returnValue(of([makeActiveMatch(1)]));
    scoreboardServiceSpy.updateScore.and.returnValue(of(makeActiveMatch(1)));
    const fixture = createFixture();
    fixture.detectChanges();

    fixture.componentInstance.onUpdateScore(1);

    expect(scoreboardServiceSpy.updateScore).toHaveBeenCalledWith(1, { homeScore: 6, awayScore: 6 });
    expect(successDialogSpy.openSuccess).toHaveBeenCalledWith(
      'Score updated successfully.',
      'Score Updated'
    );
  });

  it('calls finishMatch(...) and opens the success dialog on success', () => {
    scoreboardServiceSpy.getSummary.and.returnValue(of([makeActiveMatch(1)]));
    scoreboardServiceSpy.finishMatch.and.returnValue(of(makeActiveMatch(1)));
    const fixture = createFixture();
    fixture.detectChanges();

    fixture.componentInstance.onFinishMatch(1);

    expect(scoreboardServiceSpy.finishMatch).toHaveBeenCalledWith(1);
    expect(successDialogSpy.openSuccess).toHaveBeenCalledWith(
      'Match finished successfully.',
      'Match Finished'
    );
  });

  it('shows the empty-state message when there are no active matches', () => {
    const fixture = createFixture();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.no-active-box')).toBeTruthy();
  });
});
