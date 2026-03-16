import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { QuestionService } from '../../services/question.service';
import { environment } from '../../../environments/environment';
import { QuestionResponse } from '../../model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: false,
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {

  questions: QuestionResponse[] = [];
  page = 1;
  pageSize = 10;
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private questionService: QuestionService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadQuestions();
  }

  loadQuestions() {
    this.loading = true;
    this.questionService.getQuestions(this.page, this.pageSize).subscribe({
      next: (res) => {
        this.questions = [...(res.items ?? [])];
        this.loading = false;
        this.cd.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cd.detectChanges();
      }
    });
  }
}
