export interface QuizResult {
  totalQuestions: number;
  correctAnswers: number;
  questionResults: QuestionResult[];
}

export interface QuestionResult{
  questionId: string;
  questionText: string;
  isCorrect: boolean;
  userAnswer: string;
  correctAnswer: string;
}
