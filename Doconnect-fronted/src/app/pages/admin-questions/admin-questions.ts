import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin-questions',
  standalone: false,
  templateUrl: './admin-questions.html',
  styleUrl: './admin-questions.css',
})
export class AdminQuestionsComponent implements OnInit {

  questions: any[] = [];
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadQuestions();
  }

  loadQuestions() {
    this.loading = true;
    this.adminService.getPendingQuestions().subscribe({
      next: (res: any) => {
        this.questions = res ?? [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  approve(id: number) {
    this.adminService.approveQuestion(id).subscribe({
      next: () => {
        alert('Question Approved');
        this.loadQuestions();
      },
      error: (err) => alert(err.error?.error || 'Failed to approve')
    });
  }

  reject(id: number) {
    this.adminService.rejectQuestion(id).subscribe({
      next: () => {
        alert('Question Rejected');
        this.loadQuestions();
      },
      error: (err) => alert(err.error?.error || 'Failed to reject')
    });
  }

  delete(id: number) {
    if (!confirm('Delete this question?')) return;
    this.adminService.deleteQuestion(id).subscribe({
      next: () => {
        alert('Question Deleted');
        this.loadQuestions();
      },
      error: (err) => alert(err.error?.error || 'Failed to delete')
    });
  }
}
