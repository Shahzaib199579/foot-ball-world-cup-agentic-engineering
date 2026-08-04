import { FlagService } from './flag.service';

describe('FlagService', () => {
  let service: FlagService;

  beforeEach(() => {
    service = new FlagService();
  });

  it('returns the known flag URL for a country in the bundled list', () => {
    expect(service.getFlagUrl('Mexico')).toBe('https://flagcdn.com/w40/mx.png');
  });

  it('is case-insensitive and trims whitespace', () => {
    expect(service.getFlagUrl('  mexico  ')).toBe('https://flagcdn.com/w40/mx.png');
  });

  it('returns a fallback flag for an unknown country', () => {
    const url = service.getFlagUrl('Atlantis');
    expect(url).toContain('data:image/svg+xml');
  });
});
