import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AdminService } from '../../services/admin.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-admin-answers',
  standalone: false,
  templateUrl: './admin-answers.html',
  styleUrl: './admin-answers.css',
})
export class AdminAnswersComponent implements OnInit {

  answers: any[] = [];
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadAnswers();
  }

  loadAnswers() {
    this.loading = true;
    this.adminService.getPendingAnswers().subscribe({
      next: (res: any) => {
        this.answers = res ?? [];
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
    this.adminService.approveAnswer(id).subscribe({
      next: () => {
        alert('Answer Approved');
        this.loadAnswers();
      },
      error: (err) => alert(err.error?.error || 'Failed to approve')
    });
  }

  reject(id: number) {
    this.adminService.rejectAnswer(id).subscribe({
      next: () => {
        alert('Answer Rejected');
        this.loadAnswers();
      },
      error: (err) => alert(err.error?.error || 'Failed to reject')
    });
  }

  delete(id: number) {
    if (!confirm('Delete this answer?')) return;
    this.adminService.deleteAnswer(id).subscribe({
      next: () => {
        alert('Answer Deleted');
        this.loadAnswers();
      },
      error: (err) => alert(err.error?.error || 'Failed to delete')
    });
  }
}
