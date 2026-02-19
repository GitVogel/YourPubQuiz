import {Component, EventEmitter, Output} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {Dialog} from "primeng/dialog";
import {QuestionApiService} from "../../../core/services/question-api-service";
import {Category} from "../../../core/models/category";
import {AllDifficulties, Difficulty} from "../../../core/models/difficulty";
import {AllTypes, Type} from "../../../core/models/type";
import {Select} from "primeng/select";
import {Button} from "primeng/button";
import {InputNumber} from "primeng/inputnumber";

@Component({
  selector: 'app-quiz-dialog',
  imports: [
    Dialog,
    ReactiveFormsModule,
    Select,
    Button,
    InputNumber
  ],
  templateUrl: './quiz-dialog.component.html',
  styleUrl: './quiz-dialog.component.scss',
})
export class QuizDialogComponent {
  @Output() newQuizCreated = new EventEmitter<FormGroup>();

  displayDialog: boolean = false;

  categories: Category[] = [];
  allDifficulties = AllDifficulties;
  allTypes = AllTypes;

  /// Form group for creating a new pub quiz with validation for the amount of questions.
  /// The other fields are optional.
  newPubQuizForm = new FormGroup({
    amountOfQuestions: new FormControl<number | null>(null, Validators.required),
    category: new FormControl<Category[] | null>(null),
    difficulty: new FormControl<Difficulty | null>(null),
    type: new FormControl<Type | null>(null)
  })

  constructor(private readonly questionApiService: QuestionApiService) {
  }

  /// Opens the dialog for creating a new pub quiz and fetches the available question categories from the backend.
  public startNewPubQuiz(){
    this.newPubQuizForm.reset();
    this.questionApiService.getQuestionsCategories().subscribe(categories => {
      this.categories = categories;
    });
    this.displayDialog = true;
  }

    /// Creates a new pub quiz based on the form values and emits it to quiz-page if the form is valid.
  public createNewPubQuiz() {
    if (!this.newPubQuizForm.invalid) {
      this.newQuizCreated.emit(this.newPubQuizForm);
      this.displayDialog = false;
    }
  }
}
