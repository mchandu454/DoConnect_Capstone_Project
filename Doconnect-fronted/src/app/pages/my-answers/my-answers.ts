import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AnswerService } from '../../services/answer.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-my-answers',
  standalone: false,
  templateUrl: './my-answers.html',
  styleUrl: './my-answers.css'
})
export class MyAnswersComponent implements OnInit {

  answers: any[] = [];
  loading = true;
  baseUrl = environment.hubUrl;
  editingId: number | null = null;
  editText = '';
  editImageFile: File | null = null;
  submitting = false;

  constructor(
    private answerService: AnswerService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadAnswers();
  }

  loadAnswers() {
    this.loading = true;
    this.editingId = null;
    this.answerService.getMyAnswers().subscribe({
      next: (res: any) => {
        this.answers = Array.isArray(res) ? res : [];
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.loading = false;
        this.cdr.detectChanges();
      }
    });
  }

  startEdit(a: any) {
    this.editingId = a.answerId ?? a.AnswerId;
    this.editText = a.answerText ?? a.AnswerText ?? '';
    this.editImageFile = null;
    this.cdr.detectChanges();
  }

  cancelEdit() {
    this.editingId = null;
    this.editText = '';
    this.editImageFile = null;
    this.cdr.detectChanges();
  }

  onEditImage(e: any) {
    this.editImageFile = e.target?.files?.[0] ?? null;
    this.cdr.detectChanges();
  }

  updateAnswer() {
    if (this.editingId == null) return;
    const formData = new FormData();
    formData.append('answerText', this.editText);
    if (this.editImageFile) formData.append('image', this.editImageFile);
    this.submitting = true;
    this.answerService.updateAnswer(this.editingId, formData).subscribe({
      next: () => {
        this.submitting = false;
        this.cancelEdit();
        this.loadAnswers();
      },
      error: (err) => {
        this.submitting = false;
        alert(err.error?.error || 'Failed to update answer.');
        this.cdr.detectChanges();
      }
    });
  }

  deleteAnswer(a: any) {
    if (!confirm('Delete this answer? This cannot be undone.')) return;
    const id = a.answerId ?? a.AnswerId;
    this.answerService.deleteAnswer(id).subscribe({
      next: () => this.loadAnswers(),
      error: (err) => alert(err.error?.error || 'Failed to delete.')
    });
  }
}
