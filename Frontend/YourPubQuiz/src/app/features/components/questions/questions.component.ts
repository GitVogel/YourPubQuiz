import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Panel} from "primeng/panel";
import {RadioButton} from "primeng/radiobutton";
import {FormsModule} from "@angular/forms";
import {Button} from "primeng/button";
import {QuestionApiService} from "../../../core/services/question-api-service";
import {QuizResult} from "../../../core/models/quizResult";
import {QuizData} from "../../../core/models/quizData";
import {QuizAnswers} from "../../../core/models/quizAnswers";

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
  @Input() quizData: QuizData = { } as QuizData;
  @Input() isActive: boolean = false;
  @Output() quizResults = new EventEmitter<QuizResult>();

  selectedAnswers: Record<string, string> = {};
  quizAnswers: QuizAnswers = { id: "", answers: [] } as QuizAnswers;

  /// Checks if the submit button should be disabled based on whether all questions have been answered.
  get isSubmitDisabled(): boolean {
    return Object.keys(this.selectedAnswers).length !== this.quizData.questions.length;
  }

  constructor(private readonly questionApiService: QuestionApiService) {}

  /// Submits the selected answers to the backend for checking and emits the results to quiz-page.
  public submitAnswers() {
    this.quizAnswers.id = this.quizData.id;
    this.quizAnswers.answers = Object.entries(this.selectedAnswers).map(([id, answer]) => ({
      id,
      answer
    }));
    this.questionApiService.checkAnswer(this.quizAnswers).subscribe({
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
