import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { DashboardComponent } from './pages/dashboard/dashboard';
import { AskQuestionComponent } from './pages/ask-question/ask-question';
import { QuestionDetailComponent } from './pages/question-detail/question-detail';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';
import { AdminQuestionsComponent } from './pages/admin-questions/admin-questions';
import { AdminAnswersComponent } from './pages/admin-answers/admin-answers';
import { AdminHistoryComponent } from './pages/admin-history/admin-history';
import { MyQuestionsComponent } from './pages/my-questions/my-questions';
import { MyAnswersComponent } from './pages/my-answers/my-answers';
import { SearchComponent } from './pages/search/search';

const routes: Routes = [

{ path:'login', component: LoginComponent },
{ path:'register', component: RegisterComponent },
{ path:'dashboard', component: DashboardComponent },
{ path:'search', component: SearchComponent, canActivate:[AuthGuard] },
{ path:'ask-question', component: AskQuestionComponent },
{ path:'question-detail/:id', component: QuestionDetailComponent },
{ path:'my-questions', component: MyQuestionsComponent, canActivate:[AuthGuard] },
{ path:'my-answers', component: MyAnswersComponent, canActivate:[AuthGuard] },
{ path:'admin-dashboard', component: AdminDashboard, canActivate:[AuthGuard] },

{ path:'admin/questions', component: AdminQuestionsComponent, canActivate:[AuthGuard] },

{ path:'admin/answers', component: AdminAnswersComponent, canActivate:[AuthGuard] },

{ path:'admin/history', component: AdminHistoryComponent, canActivate:[AuthGuard] },
{ path:'', redirectTo:'login', pathMatch:'full' },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}