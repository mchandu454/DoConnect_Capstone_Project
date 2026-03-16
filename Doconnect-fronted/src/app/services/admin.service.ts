import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AnswerResponse, QuestionResponse } from '../model';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  private apiUrl = environment.apiUrl + '/admin';

  constructor(private http: HttpClient) {}

  // QUESTIONS

  getPendingQuestions(){
    return this.http.get<QuestionResponse[]>(`${this.apiUrl}/pending-questions`);
  }

  approveQuestion(id: number){
    return this.http.put(`${this.apiUrl}/approve-question/${id}`, {});
  }

  rejectQuestion(id: number){
    return this.http.put(`${this.apiUrl}/reject-question/${id}`, {});
  }

  deleteQuestion(id: number){
    return this.http.delete(`${this.apiUrl}/delete-question/${id}`);
  }


  // ANSWERS

  getPendingAnswers(){
    return this.http.get<AnswerResponse[]>(`${this.apiUrl}/pending-answers`);
  }

  approveAnswer(id: number){
    return this.http.put(`${this.apiUrl}/approve-answer/${id}`, {});
  }

  rejectAnswer(id: number){
    return this.http.put(`${this.apiUrl}/reject-answer/${id}`, {});
  }

  deleteAnswer(id: number){
    return this.http.delete(`${this.apiUrl}/delete-answer/${id}`);
  }


  // HISTORY

  getHistoryQuestions(){
    return this.http.get<QuestionResponse[]>(`${this.apiUrl}/history-questions`);
  }

  getHistoryAnswers(){
    return this.http.get<AnswerResponse[]>(`${this.apiUrl}/history-answers`);
  }

}