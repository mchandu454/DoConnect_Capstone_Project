import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { QuestionService } from '../../services/question.service';
import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-dashboard',
  standalone: false,
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent implements OnInit {

  questions:any[] = [];
  page = 1;
  pageSize = 10;
  loading = true;
  baseUrl = environment.hubUrl;

  constructor(
    private questionService: QuestionService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(){
    console.log("Dashboard loaded");
    this.loadQuestions();
  }

  loadQuestions(){

    this.loading = true;

    this.questionService.getQuestions(this.page,this.pageSize)
      .subscribe((res:any)=>{

        console.log("FULL RESPONSE:", res);

        this.questions = [...(res.items ?? [])];

        this.loading = false;

        this.cd.detectChanges();   // 🔑 forces UI refresh

      });

  }

}