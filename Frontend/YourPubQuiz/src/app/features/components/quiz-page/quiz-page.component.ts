import {Component, ViewChild} from '@angular/core';
import {FormGroup, FormsModule} from "@angular/forms";
import {QuestionApiService} from "../../../core/services/question-api-service";
import {Button} from "primeng/button";
import {QuizDialogComponent} from "../quiz-dialog/quiz-dialog.component";
import {Question} from "../../../core/models/question";
import {QuestionsComponent} from "../questions/questions.component";
import {ResultsDialogComponent} from "../results-dialog/results-dialog.component";
import {QuizResult} from "../../../core/models/quizResult";

@Component({
  selector: 'app-quiz-page',
  imports: [
    FormsModule,
    Button,
    QuizDialogComponent,
    QuestionsComponent,
    ResultsDialogComponent
  ],
  templateUrl: './quiz-page.component.html',
  styleUrl: './quiz-page.component.scss',
})
export class QuizPageComponent {
  @ViewChild(QuizDialogComponent) quizDialog!: QuizDialogComponent;
  @ViewChild(ResultsDialogComponent) resultsDialog!: ResultsDialogComponent;

  quizIsActive: boolean = false;
  showQuizResultDialog: boolean = false;
  questions: Question[] = [];
  quizResult: QuizResult = {} as QuizResult;

  constructor(private readonly questionApiService: QuestionApiService) {
  }

  /// Starts a new pub quiz by opening the quiz dialog and resetting the quiz state.
  public newPubQuiz() {
    this.quizDialog.startNewPubQuiz();
    this.showQuizResultDialog = false;
  }

    /// Fetches new questions from the backend based on the quiz settings and activates the quiz.
  public getNewQuestions(quizSetting: FormGroup)
  {
    this.questionApiService.getQuestions(quizSetting).subscribe(questions => {
      this.questions = questions;
      this.quizIsActive = true;
    });
  }

    /// Shows the quiz results by opening the results dialog and resetting the quiz state.
  public showQuizResults(result: QuizResult) {
    this.quizResult = result;
    this.questions = [];
    this.quizIsActive = false;
    this.showQuizResultDialog = true;
  }
}
