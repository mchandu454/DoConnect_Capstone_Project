import { Component } from '@angular/core';
import { QuestionService } from '../../services/question.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-ask-question',
  standalone: false,
  templateUrl: './ask-question.html',
  styleUrl: './ask-question.css'
})
export class AskQuestionComponent {

  title = '';
  description = '';
  imageFile: File | null = null;
  submitting = false;

  constructor(
    private questionService: QuestionService,
    private router: Router
  ) {}

  onFileSelected(event:any){
    this.imageFile = event.target.files[0];
  }

  submitQuestion() {
    const formData = new FormData();
    formData.append("title", this.title);
    formData.append("description", this.description);
    if (this.imageFile) {
      formData.append("image", this.imageFile);
    }

    this.submitting = true;
    this.questionService.createQuestion(formData).subscribe({
      next: () => {
        alert("Question submitted successfully.");
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.submitting = false;
        console.error(err);
        alert(err.error?.error || "Failed to submit question.");
      }
    });
  }

}