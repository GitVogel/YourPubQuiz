import {Component, Input} from '@angular/core';
import {Dialog} from "primeng/dialog";
import {QuizResult} from "../../../core/models/quizResult";
import {TableModule} from "primeng/table";
import {Button} from "primeng/button";

@Component({
  selector: 'app-results-dialog',
  imports: [
    Dialog,
    TableModule,
    Button
  ],
  templateUrl: './results-dialog.component.html',
  styleUrl: './results-dialog.component.scss',
})
export class ResultsDialogComponent {
  @Input() showResults: boolean = false;
  @Input() quizResult: QuizResult = {} as QuizResult;

  constructor() {
  }

  /// Closes the results dialog.
  public closeDialog() {
    this.showResults = false;
  }
}
