import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideNoopAnimations } from '@angular/platform-browser/animations';
import { SidenavComponent } from './sidenav.component';

describe('SidenavComponent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SidenavComponent],
      providers: [provideRouter([]), provideNoopAnimations()]
    });
  });

  it('renders 3 nav links: Summary, History, Matches', () => {
    const fixture = TestBed.createComponent(SidenavComponent);
    fixture.detectChanges();

    const links: HTMLAnchorElement[] = Array.from(fixture.nativeElement.querySelectorAll('a.nav-item'));
    expect(links.length).toBe(3);
    const hrefs = links.map((l) => l.getAttribute('href'));
    expect(hrefs).toEqual(['/summary', '/history', '/matches']);
  });

  it('renders the app title in the toolbar', () => {
    const fixture = TestBed.createComponent(SidenavComponent);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.brand-title').textContent).toContain('World Cup Scoreboard');
  });
});
