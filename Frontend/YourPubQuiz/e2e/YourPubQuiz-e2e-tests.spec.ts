import { test, expect } from '@playwright/test';

test('Default quiz page', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Your PubQuiz', exact: true })).toBeVisible();
  await expect(page.getByRole('heading', { name: 'The only place for all your PubQuizing needs', exact: true })).toBeVisible();
  await expect(page.getByRole('button', { name: 'New PubQuiz' })).toBeVisible();
});

test('Create new basic quiz', async ({ page }) => {
  await page.goto('/');

  await page.getByRole('button', { name: 'New PubQuiz' }).click();

  const dialog = page.locator('p-dialog').filter({ has: page.getByText('New PubQuiz settings') });
  await dialog.isVisible();
  const amount = dialog.locator('p-inputNumber[formControlName="amountOfQuestions"] input');
  await amount.fill('5');
  await amount.press('Tab');

  await expect(dialog.getByRole('button', { name: 'Begin PubQuiz' })).toBeEnabled();

  const [response] = await Promise.all([
    page.waitForResponse('http://localhost:5025/Quiz/GetQuestions?questionAmount=5&category=&difficulty=&type='),
    dialog.getByRole('button', { name: 'Begin PubQuiz' }).click(),
  ]);

  expect(response.status()).toBe(200);
  expect(await response.json()).toHaveLength(5);
});

test('Create new quiz with settings', async ({ page }) => {
  await page.goto('/');

  const [categories] = await Promise.all([
    page.waitForResponse('http://localhost:5025/Quiz/GetCategories'),
    page.getByRole('button', { name: 'New PubQuiz' }).click()
  ]);

  const dialog = page.locator('p-dialog').filter({ has: page.getByText('New PubQuiz settings') });
  await dialog.isVisible();
  const amount = dialog.locator('p-inputNumber[formControlName="amountOfQuestions"] input');
  await amount.fill('5');
  await amount.press('Tab');

  const categorySelect = dialog.locator('p-select[formControlName="category"]');
  await categorySelect.click();

  const categoriesData = await categories.json();
  await page.getByRole('option', { name: categoriesData[0].name }).click();

  const difficultySelect = dialog.locator('p-select[formControlName="difficulty"]');
  await difficultySelect.click();
  await page.getByRole('option', { name: 'Easy' }).click();

  const typeSelect = dialog.locator('p-select[formControlName="type"]');
  await typeSelect.click();
  await page.getByRole('option', { name: 'Multiple Choice' }).click();

  await expect(dialog.getByRole('button', { name: 'Begin PubQuiz' })).toBeEnabled();

  const [response] = await Promise.all([
    page.waitForResponse(`http://localhost:5025/Quiz/GetQuestions?questionAmount=5&category=${categoriesData[0].id}&difficulty=easy&type=multiple`),
    dialog.getByRole('button', { name: 'Begin PubQuiz' }).click(),
  ]);

  expect(response.status()).toBe(200);
  expect(await response.json()).toHaveLength(5);
});

test('Create and complete quiz', async ({ page }) => {
  await page.goto('/');

  const newQuizButton = page.getByRole('button', { name: 'New PubQuiz' })
  await expect(newQuizButton).toBeVisible();
  await newQuizButton.click();

  const settingDialog = page.locator('p-dialog').filter({ has: page.getByText('New PubQuiz settings') });
  await settingDialog.isVisible();
  const amount = settingDialog.locator('p-inputNumber[formControlName="amountOfQuestions"] input');
  await amount.fill('5');
  await amount.press('Tab');

  const beginButton = settingDialog.getByRole('button', { name: 'Begin PubQuiz' });
  await expect(beginButton).toBeEnabled();

  const [questionResponse] = await Promise.all([
    page.waitForResponse('http://localhost:5025/Quiz/GetQuestions?questionAmount=5&category=&difficulty=&type='),
    beginButton.click(),
  ]);
  const questions = await questionResponse.json();

  expect(questionResponse.status()).toBe(200);
  expect(questions).toHaveLength(5);

  await expect(newQuizButton).toBeHidden();

  const panels = page.locator('p-panel');
  await expect(panels).toHaveCount(questions.length);

  const submitButton = page.getByRole('button', { name: 'Submit Answers' });
  await expect(submitButton).toBeDisabled();

  for (let i = 0; i < questions.length; i++) {
    const panel = panels.nth(i);
    await expect(panel).toContainText(`Question ${i + 1}`);
    await expect(panel).toContainText(questions[i].question);

    const answer = questions[i].possibleAnswer[0];
    await panel.getByText(answer, { exact: true }).click();
  }

  await expect(submitButton).toBeEnabled();

  const [answersResponse] = await Promise.all([
    page.waitForResponse('http://localhost:5025/Quiz/CheckAnswers'),
    submitButton.click(),
  ]);
  const answers = await answersResponse.json();

  expect(answersResponse.status()).toBe(200);
  expect(answers).toHaveProperty('totalQuestions', questions.length);
  expect(answers).toHaveProperty('correctAnswers');
  expect(Array.isArray(answers.questionResults)).toBe(true);
  expect(answers.questionResults).toHaveLength(questions.length);

  const resultsDialog = page.locator('p-dialog').filter({ has: page.getByText(`Results: ${answers.correctAnswers}/${answers.totalQuestions} Correct`) });
  await resultsDialog.isVisible();

  const closerResultsButton = resultsDialog.getByRole('button', { name: 'Close results' });
  await expect(closerResultsButton).toBeEnabled();
  await closerResultsButton.click();

  await expect(newQuizButton).toBeVisible();
});
