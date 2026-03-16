import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { forkJoin } from 'rxjs';
import { AdminService } from '../../services/admin.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin-history',
  standalone: false,
  templateUrl: './admin-history.html',
  styleUrl: './admin-history.css',
})
export class AdminHistoryComponent implements OnInit {

  historyQuestions: any[] = [];
  historyAnswers: any[] = [];
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadHistory();
  }

  loadHistory() {
    this.loading = true;
    forkJoin({
      questions: this.adminService.getHistoryQuestions(),
      answers: this.adminService.getHistoryAnswers()
    }).subscribe({
      next: (res: any) => {
        this.historyQuestions = (res.questions ?? []).map((q: any) => ({ ...q, itemType: 'Question' }));
        this.historyAnswers = (res.answers ?? []).map((a: any) => ({ ...a, itemType: 'Answer' }));
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }
}
