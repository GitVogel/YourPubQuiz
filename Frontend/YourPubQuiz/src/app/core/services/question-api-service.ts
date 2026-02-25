import { Injectable } from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Category} from "../models/category";
import {BackendSettings} from "../settings/backendSettings";
import {FormGroup} from "@angular/forms";
import {Answer} from "../models/answer";
import {QuizData} from "../models/quizData";

@Injectable({
  providedIn: 'root'
})
export class QuestionApiService {
  private  baseUrl = BackendSettings.apiUrl

  constructor(
    private readonly httpClient: HttpClient,
  ) { }

  public getQuestions(quizSettings: FormGroup) {
    let params = new HttpParams()
      .set('questionAmount', quizSettings.get('amountOfQuestions')?.value)
      .set('category', quizSettings.get('category')?.value?.id ?? '')
      .set('difficulty', quizSettings.get('difficulty')?.value?.enumValue ?? '')
      .set('type', quizSettings.get('type')?.value?.enumValue ?? '');

    const options = {
      params: params,
    };

    return this.httpClient
      .get<QuizData>(this.baseUrl + '/GetQuestions', options);
  }

  public getQuestionsCategories() {
    return this.httpClient
      .get<Category[]>(this.baseUrl + '/GetCategories');
  }

  public checkAnswer(answers: Answer[]) {
    return this.httpClient
      .post(this.baseUrl + '/CheckAnswers', answers);
  }
}
