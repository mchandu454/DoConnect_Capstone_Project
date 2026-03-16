import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { QuestionService } from '../../services/question.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-search',
  standalone: false,
  templateUrl: './search.html',
  styleUrl: './search.css'
})
export class SearchComponent implements OnInit {

  query = '';
  questions: any[] = [];
  total = 0;
  page = 1;
  pageSize = 10;
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private route: ActivatedRoute,
    private questionService: QuestionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.query = (params['q'] || '').trim();
      this.page = 1;
      if (this.query) {
        this.runSearch();
      } else {
        this.questions = [];
        this.total = 0;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  runSearch() {
    if (!this.query.trim()) return;
    this.loading = true;
    this.questionService.searchQuestions(this.query, this.page, this.pageSize).subscribe({
      next: (res: any) => {
        this.questions = res?.items ?? [];
        this.total = res?.total ?? 0;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.questions = [];
        this.total = 0;
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
