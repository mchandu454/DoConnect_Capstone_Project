import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { QuestionService } from '../../services/question.service';
import { AnswerService } from '../../services/answer.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-question-detail',
  standalone: false,
  templateUrl: './question-detail.html',
  styleUrl: './question-detail.css'
})
export class QuestionDetailComponent implements OnInit {

  question:any;
  answers:any[] = [];

  answerText = '';
  imageFile: File | null = null;

  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private route: ActivatedRoute,
    private questionService: QuestionService,
    private answerService: AnswerService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(){

    const id = this.route.snapshot.paramMap.get('id');

    if(id){
      this.loadQuestion(+id);
      this.loadAnswers(+id);
    }

  }

  loadQuestion(id:number){

    this.questionService.getQuestionById(id)
      .subscribe((res:any)=>{

        console.log("Question:", res);

        this.question = res;

        this.loading = false;

        this.cd.detectChanges();

      });

  }

  loadAnswers(questionId:number){

    this.answerService.getAnswers(questionId)
      .subscribe((res:any)=>{

        console.log("Answers:", res);

        this.answers = [...(res ?? [])];

        this.cd.detectChanges();

      });

  }

  onFileSelected(event:any){
    this.imageFile = event.target.files[0];
  }

  submitAnswer(){

    const formData = new FormData();

    formData.append("questionId", this.question.questionId);
    formData.append("answerText", this.answerText);

    if(this.imageFile){
      formData.append("image", this.imageFile);
    }

    this.answerService.createAnswer(formData)
      .subscribe(()=>{

        alert("Answer submitted");

        this.answerText = '';

        this.loadAnswers(this.question.questionId);

      });

  }

}