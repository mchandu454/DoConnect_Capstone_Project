import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {

  username = '';
  password = '';

  constructor(private authService: AuthService, private router: Router) {}

  register() {

    console.log("Register Button Clicked");

    const data = {
      username: this.username,
      password: this.password
    };

    this.authService.register(data).subscribe({

      next: (res:any) => {

        console.log("Register Success:", res);

        alert("Registration Successful");

        this.router.navigate(['/login']);

      },

      error: (err) => {

        console.error(err);

        alert("Registration Failed");

      }

    });

  }

}