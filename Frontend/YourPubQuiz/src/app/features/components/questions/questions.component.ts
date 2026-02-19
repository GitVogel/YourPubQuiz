import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Question} from "../../../core/models/question";
import {Panel} from "primeng/panel";
import {RadioButton} from "primeng/radiobutton";
import {FormsModule} from "@angular/forms";
import {Button} from "primeng/button";
import {Answer} from "../../../core/models/answer";
import {QuestionApiService} from "../../../core/services/question-api-service";
import {QuizResult} from "../../../core/models/quizResult";

@Component({
  selector: 'app-questions',
  imports: [
    FormsModule,
    Panel,
    RadioButton,
    Button
  ],
  templateUrl: './questions.component.html',
  styleUrl: './questions.component.scss',
})
export class QuestionsComponent{
  @Input() allQuestions: Question[] = [];
  @Input() isActive: boolean = false;
  @Output() quizResults = new EventEmitter<QuizResult>();

  selectedAnswers: Record<string, string> = {};
  answers: Answer[] = []

  /// Checks if the submit button should be disabled based on whether all questions have been answered.
  get isSubmitDisabled(): boolean {
    return Object.keys(this.selectedAnswers).length !== this.allQuestions.length;
  }

  constructor(private readonly questionApiService: QuestionApiService) {}

  /// Submits the selected answers to the backend for checking and emits the results to quiz-page.
  public submitAnswers() {
    this.answers = Object.entries(this.selectedAnswers).map(([id, answer]) => ({
      id,
      answer
    }));
    this.questionApiService.checkAnswer(this.answers).subscribe({
      next: (response) =>
      {
        this.selectedAnswers = {};
        this.quizResults.emit(response as QuizResult);
      },
      error: (error) =>
      {
        console.error('Error:', error)
      }
    });
  }
}
