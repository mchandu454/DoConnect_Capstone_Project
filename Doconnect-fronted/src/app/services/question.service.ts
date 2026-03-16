import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { PaginatedResult, QuestionDetail, QuestionResponse } from '../model';

@Injectable({
  providedIn: 'root'
})
export class QuestionService {

  private apiUrl = environment.apiUrl + '/questions';

  constructor(private http: HttpClient) {}

  getQuestions(page: number, pageSize: number) {
    return this.http.get<PaginatedResult<QuestionResponse>>(
      `${this.apiUrl}?page=${page}&pageSize=${pageSize}`
    );
  }

  searchQuestions(q: string, page: number = 1, pageSize: number = 10) {
    return this.http.get<PaginatedResult<QuestionResponse> & { query?: string }>(
      `${this.apiUrl}/search?q=${encodeURIComponent(q)}&page=${page}&pageSize=${pageSize}`
    );
  }

  createQuestion(formData:FormData){
    return this.http.post(this.apiUrl, formData);
  }

  getQuestionById(id: number) {
    return this.http.get<QuestionDetail>(`${this.apiUrl}/${id}`);
  }

  getMyQuestions() {
    return this.http.get<QuestionResponse[]>(`${this.apiUrl}/my-questions`);
  }

  updateQuestion(id: number, formData: FormData) {
    return this.http.put(`${this.apiUrl}/${id}`, formData);
  }

  deleteQuestion(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}