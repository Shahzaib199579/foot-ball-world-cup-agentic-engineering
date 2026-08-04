import { test, expect, Page } from '@playwright/test';

async function selectCountryOption(page: Page, formControlName: string, countryName: string) {
  await page.locator(`mat-select[formcontrolname="${formControlName}"]`).click();
  await page.locator(`.cdk-overlay-container mat-option`).filter({ hasText: countryName }).first().click();
}

async function expectSuccessDialog(page: Page, expectedText: string) {
  const dialog = page.locator('app-success-dialog');
  await expect(dialog).toBeVisible();
  await expect(dialog).toContainText(expectedText);
  await dialog.locator('.ok-btn').click();
  await expect(dialog).not.toBeVisible();
}

// `location` is required and reset to blank after every successful start (see
// matches.component.ts) precisely so two different match pairs never collide on the backend's
// "same location + same time" in-progress uniqueness rule — so every start here fills a
// location derived from the team pair, guaranteeing distinctness across the whole suite.
async function fillLocation(page: Page, home: string, away: string) {
  await page.locator('input[formcontrolname="location"]').fill(`${home} vs ${away} Stadium`);
}

async function startMatch(page: Page, home: string, away: string) {
  await selectCountryOption(page, 'homeTeam', home);
  await selectCountryOption(page, 'awayTeam', away);
  await fillLocation(page, home, away);
  await page.locator('button.start-btn').click();
  await expectSuccessDialog(page, 'Match started successfully.');
}

async function updateScore(page: Page, activeCard: ReturnType<Page['locator']>, homeScore: string, awayScore: string) {
  await activeCard.locator('.score-number-input').nth(0).fill(homeScore);
  await activeCard.locator('.score-number-input').nth(1).fill(awayScore);
  await activeCard.locator('.update-score-btn').click();
  await expectSuccessDialog(page, 'Score updated successfully.');
}

async function finishMatch(page: Page, activeCard: ReturnType<Page['locator']>) {
  await activeCard.locator('.finish-match-btn').click();
  await expectSuccessDialog(page, 'Match finished successfully.');
}

test.describe('World Cup Scoreboard Frontend E2E', () => {

  test('1. should display sleek UI header and left side navigation tabs', async ({ page }) => {
    await page.goto('http://localhost:4200');
    await expect(page.locator('.brand-title')).toHaveText('World Cup Scoreboard');
    await expect(page.locator('a[routerlink="/summary"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/history"]')).toBeVisible();
    await expect(page.locator('a[routerlink="/matches"]')).toBeVisible();
  });

  test('2. should start Spain vs Brazil, update score, finish match, and verify error + success modals', async ({ page }) => {
    await page.goto('http://localhost:4200');
    await page.click('a[routerlink="/matches"]');

    // Select Spain (Home) & Brazil (Away)
    await startMatch(page, 'Spain', 'Brazil');

    // Verify active match card appears
    await expect(page.locator('.active-match-card').filter({ hasText: 'Spain' })).toBeVisible();

    // Switch to Summary and verify Spain vs Brazil live row
    await page.click('a[routerlink="/summary"]');
    await expect(page.locator('app-match-row').filter({ hasText: 'Spain' })).toBeVisible();

    // Attempt starting duplicate match with Spain -> Verify error modal (no success modal)
    await page.click('a[routerlink="/matches"]');
    await selectCountryOption(page, 'homeTeam', 'Spain');
    await selectCountryOption(page, 'awayTeam', 'Argentina');
    await fillLocation(page, 'Spain', 'Argentina');
    await page.locator('button.start-btn').click();

    // Error modal popup
    await expect(page.locator('app-error-dialog')).toBeVisible();
    await expect(page.locator('.error-message')).toContainText('Start did not succeed');
    await page.locator('.dismiss-btn').click();
    await expect(page.locator('app-error-dialog')).not.toBeVisible();
    await expect(page.locator('app-success-dialog')).not.toBeVisible();

    // Update Score to Spain 10 - Brazil 2
    const activeSpainCard = page.locator('.active-match-card').filter({ hasText: 'Spain' });
    await updateScore(page, activeSpainCard, '10', '2');

    // Verify score on Summary tab
    await page.click('a[routerlink="/summary"]');
    await expect(page.locator('app-match-row').filter({ hasText: 'Spain' })).toContainText('10');
    await expect(page.locator('app-match-row').filter({ hasText: 'Spain' })).toContainText('2');

    // Attempt invalid score decrease (Spain 5) -> Error modal, no success modal
    await page.click('a[routerlink="/matches"]');
    await activeSpainCard.locator('.score-number-input').nth(0).fill('5');
    await activeSpainCard.locator('.update-score-btn').click();

    await expect(page.locator('app-error-dialog')).toBeVisible();
    await expect(page.locator('.error-message')).toContainText('Score update rejected');
    await page.locator('.dismiss-btn').click();
    await expect(page.locator('app-success-dialog')).not.toBeVisible();

    // Finish Spain vs Brazil match
    await finishMatch(page, activeSpainCard);

    // Verify match appears as FINISHED in History tab
    await page.click('a[routerlink="/history"]');
    await expect(page.locator('app-match-row').filter({ hasText: 'Spain' })).toBeVisible();
    await expect(page.locator('app-match-row').filter({ hasText: 'Spain' })).toContainText('FINISHED');
  });

  test('3. should replay the brief\'s full worked example and verify exact Summary ordering', async ({ page }) => {
    await page.goto('http://localhost:4200');
    await page.click('a[routerlink="/matches"]');

    // Mexico 0-Canada 5, Spain 10-Brazil 2, Germany 2-France 2, Uruguay 6-Italy 6, Argentina 3-Australia 1
    await startMatch(page, 'Mexico', 'Canada');
    await startMatch(page, 'Spain', 'Brazil');
    await startMatch(page, 'Germany', 'France');
    await startMatch(page, 'Uruguay', 'Italy');
    await startMatch(page, 'Argentina', 'Australia');

    const mexCard = page.locator('.active-match-card').filter({ hasText: 'Mexico' });
    await updateScore(page, mexCard, '0', '5');

    const spaCard = page.locator('.active-match-card').filter({ hasText: 'Spain' });
    await updateScore(page, spaCard, '10', '2');

    const gerCard = page.locator('.active-match-card').filter({ hasText: 'Germany' });
    await updateScore(page, gerCard, '2', '2');

    const uruCard = page.locator('.active-match-card').filter({ hasText: 'Uruguay' });
    await updateScore(page, uruCard, '6', '6');

    const argCard = page.locator('.active-match-card').filter({ hasText: 'Argentina' });
    await updateScore(page, argCard, '3', '1');

    // Expected order (brief's worked example): Uruguay 6-Italy 6, Spain 10-Brazil 2,
    // Mexico 0-Canada 5, Argentina 3-Australia 1, Germany 2-France 2
    await page.click('a[routerlink="/summary"]');
    const rows = page.locator('app-match-row');
    await expect(rows).toHaveCount(5);
    await expect(rows.nth(0)).toContainText('Uruguay');
    await expect(rows.nth(0)).toContainText('Italy');
    await expect(rows.nth(1)).toContainText('Spain');
    await expect(rows.nth(1)).toContainText('Brazil');
    await expect(rows.nth(2)).toContainText('Mexico');
    await expect(rows.nth(2)).toContainText('Canada');
    await expect(rows.nth(3)).toContainText('Argentina');
    await expect(rows.nth(3)).toContainText('Australia');
    await expect(rows.nth(4)).toContainText('Germany');
    await expect(rows.nth(4)).toContainText('France');
  });

  test('4. should paginate History at 10 entries per page, newest activity first', async ({ page }) => {
    await page.goto('http://localhost:4200');
    await page.click('a[routerlink="/matches"]');

    // Tests 2/3 leave Mexico/Canada/Spain/Brazil/Germany/France/Uruguay/Italy/Argentina/
    // Australia all still in-progress, so those team names aren't available here. Cycle a
    // small pool of the remaining free countries, finishing each match immediately so its
    // teams are free again next round — guarantees at least 10 new history entries without
    // running out of distinct team names or colliding with any still-in-progress match.
    const freePairs: [string, string][] = [
      ['England', 'Portugal'], ['Netherlands', 'Croatia'], ['Japan', 'USA'], ['Morocco', 'Senegal']
    ];
    for (let round = 0; round < 3; round++) {
      for (const [home, away] of freePairs) {
        await startMatch(page, home, away);
        const card = page.locator('.active-match-card').filter({ hasText: home });
        await finishMatch(page, card);
      }
    }

    await page.click('a[routerlink="/history"]');

    // Page 1 always shows exactly 10 (the API caps each page at 10), most-recently-active first.
    await expect(page.locator('app-match-row')).toHaveCount(10);
    await expect(page.locator('.page-indicator')).toContainText('Page 1');
    const normalize = (s: string) => s.replace(/\s+/g, ' ').trim();
    const firstRowOnPage1 = normalize(await page.locator('app-match-row').first().innerText());
    await expect(page.locator('.nav-page-btn').filter({ hasText: 'Previous Page' })).toBeDisabled();
    await expect(page.locator('.nav-page-btn').filter({ hasText: 'Next Page' })).toBeEnabled();

    // Next Page moves forward to a different (non-overlapping) set of matches.
    await page.locator('.nav-page-btn').filter({ hasText: 'Next Page' }).click();
    await expect(page.locator('.page-indicator')).toContainText('Page 2');
    await expect(async () => {
      const firstRowOnPage2 = normalize(await page.locator('app-match-row').first().innerText());
      expect(firstRowOnPage2).not.toBe(firstRowOnPage1);
    }).toPass();

    // Previous Page returns to exactly the same page 1 content.
    await page.locator('.nav-page-btn').filter({ hasText: 'Previous Page' }).click();
    await expect(page.locator('.page-indicator')).toContainText('Page 1');
    await expect(async () => {
      const firstRowBackOnPage1 = normalize(await page.locator('app-match-row').first().innerText());
      expect(firstRowBackOnPage1).toBe(firstRowOnPage1);
    }).toPass();
  });

});
