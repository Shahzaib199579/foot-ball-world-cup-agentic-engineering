import { TestBed } from '@angular/core/testing';
import { CountryCardComponent } from './country-card.component';
import { FlagService } from '../../../core/services/flag.service';

describe('CountryCardComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [CountryCardComponent]
    });
  });

  it('renders the flag, country name, and score', () => {
    const fixture = TestBed.createComponent(CountryCardComponent);
    fixture.componentInstance.countryName = 'Mexico';
    fixture.componentInstance.score = 3;
    fixture.detectChanges();

    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.country-name')?.textContent).toContain('Mexico');
    expect(el.querySelector('.score-badge')?.textContent).toContain('3');
    expect(el.querySelector('.country-flag')?.getAttribute('src')).toBe(
      TestBed.inject(FlagService).getFlagUrl('Mexico')
    );
  });

  it('defaults to the "home" side styling class', () => {
    const fixture = TestBed.createComponent(CountryCardComponent);
    fixture.detectChanges();
    const card = fixture.nativeElement.querySelector('mat-card');
    expect(card.classList).toContain('home');
  });
});
