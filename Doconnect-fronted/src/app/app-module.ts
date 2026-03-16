import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { HttpClientModule, provideHttpClient, withInterceptors } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { authInterceptor } from './interceptors/auth.interceptor';
import { HeaderComponent } from './components/header/header';
import { LoginComponent } from './pages/login/login';
import { RegisterComponent } from './pages/register/register';
import { DashboardComponent } from './pages/dashboard/dashboard';
import { AdminDashboard } from './pages/admin-dashboard/admin-dashboard';
import { AskQuestionComponent } from './pages/ask-question/ask-question';
import { QuestionDetailComponent } from './pages/question-detail/question-detail';
import { AdminQuestionsComponent } from './pages/admin-questions/admin-questions';
import { AdminAnswersComponent } from './pages/admin-answers/admin-answers';
import { AdminHistoryComponent } from './pages/admin-history/admin-history';
import { MyQuestionsComponent } from './pages/my-questions/my-questions';
import { MyAnswersComponent } from './pages/my-answers/my-answers';
import { SearchComponent } from './pages/search/search';

@NgModule({
  declarations: [
    App,
    HeaderComponent,
    LoginComponent,
    RegisterComponent,
    DashboardComponent,
    AdminDashboard,
    AskQuestionComponent,
    QuestionDetailComponent,
    AdminQuestionsComponent,
    AdminAnswersComponent,
    AdminHistoryComponent,
    MyQuestionsComponent,
    MyAnswersComponent,
    SearchComponent,
  ],
  imports: [BrowserModule, AppRoutingModule, HttpClientModule, FormsModule],
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor])),
  ],
  bootstrap: [App],
})
export class AppModule {}
