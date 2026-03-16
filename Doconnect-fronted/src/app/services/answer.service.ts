import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AnswerResponse } from '../model';

@Injectable({
  providedIn: 'root'
})
export class AnswerService {

  private apiUrl = environment.apiUrl + '/answers';

  constructor(private http: HttpClient) {}

  getAnswers(questionId:number){
    return this.http.get<AnswerResponse[]>(`${this.apiUrl}/question/${questionId}`);
  }

  createAnswer(formData: FormData) {
    return this.http.post(this.apiUrl, formData);
  }

  getMyAnswers() {
    return this.http.get<AnswerResponse[]>(`${this.apiUrl}/my-answers`);
  }

  updateAnswer(id: number, formData: FormData) {
    return this.http.put(`${this.apiUrl}/${id}`, formData);
  }

  deleteAnswer(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}