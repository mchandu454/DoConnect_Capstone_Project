import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {

  username = '';
  password = '';

  constructor(private authService:AuthService, private router:Router){}

  login(){

    console.log("Login Button Clicked");

    const data = {
      username: this.username,
      password: this.password
    };

    this.authService.login(data).subscribe({

      next:(res:any)=>{

        console.log("Login Success:", res);

        const token = res.token || res.Token;

        const role = res.role || res.Role || '';
        localStorage.setItem("token", token);
        localStorage.setItem("role", role);
        localStorage.setItem("username", res.username || res.Username);
        localStorage.setItem("userId", res.userId || res.UserId);

        alert("Login Successful");

        if (role === 'Admin') {
          this.router.navigate(['/admin-dashboard']);
        } else {
          this.router.navigate(['/dashboard']);
        }

      },

      error:(err)=>{

        console.error(err);

        alert("Invalid username or password");

      }

    });

  }

}