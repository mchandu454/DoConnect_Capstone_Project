import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { QuestionService } from '../../services/question.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-my-questions',
  standalone: false,
  templateUrl: './my-questions.html',
  styleUrl: './my-questions.css'
})
export class MyQuestionsComponent implements OnInit {

  questions: any[] = [];
  loading = true;
  baseUrl = environment.hubUrl;
  editingId: number | null = null;
  editTitle = '';
  editDescription = '';
  editImageFile: File | null = null;
  submitting = false;

  constructor(
    private questionService: QuestionService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadQuestions();
  }

  loadQuestions() {
    this.loading = true;
    this.editingId = null;
    this.questionService.getMyQuestions().subscribe({
      next: (res: any) => {
        this.questions = Array.isArray(res) ? res : [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  startEdit(q: any) {
    this.editingId = q.questionId;
    this.editTitle = q.title || q.Title || '';
    this.editDescription = q.description || q.Description || '';
    this.editImageFile = null;
    this.cdr.detectChanges();
  }

  cancelEdit() {
    this.editingId = null;
    this.editTitle = '';
    this.editDescription = '';
    this.editImageFile = null;
    this.cdr.detectChanges();
  }

  onEditImage(e: any) {
    this.editImageFile = e.target?.files?.[0] ?? null;
    this.cdr.detectChanges();
  }

  updateQuestion() {
    if (this.editingId == null) return;
    const formData = new FormData();
    formData.append('title', this.editTitle);
    formData.append('description', this.editDescription);
    if (this.editImageFile) formData.append('image', this.editImageFile);
    this.submitting = true;
    this.questionService.updateQuestion(this.editingId, formData).subscribe({
      next: () => {
        this.submitting = false;
        this.cancelEdit();
        this.loadQuestions();
      },
      error: (err) => {
        this.submitting = false;
        alert(err.error?.error || 'Failed to update question.');
        this.cdr.detectChanges();
      }
    });
  }

  deleteQuestion(q: any) {
    if (!confirm('Delete this question? This cannot be undone.')) return;
    const id = q.questionId ?? q.QuestionId;
    this.questionService.deleteQuestion(id).subscribe({
      next: () => this.loadQuestions(),
      error: (err) => alert(err.error?.error || 'Failed to delete.')
    });
  }
}
