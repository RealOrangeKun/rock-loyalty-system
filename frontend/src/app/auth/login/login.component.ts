import { Component, NgZone, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { finalize, Observable } from 'rxjs';
import { UserInterface } from '../../shared/responseInterface/user.get.response.interface';
import { FacebookAuthService } from '../social-login/facebook-auth.service';
import { GoogleAuthService } from '../social-login/google-auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent implements OnInit {
  @ViewChild('loginForm') form: NgForm;
  loading: boolean = false;
  loadingMessage: string = '';
  errorMsg: string = '';
  constructor(
    private authService: AuthService,
    private facebookAuth: FacebookAuthService,
    private googleAuth: GoogleAuthService,
    private router: Router
  ) { }
  onSubmit() {
    const phoneEmailField: string = this.form.value.phone;
    const password: string = this.form.value.password;
    let loginObs: Observable<UserInterface>;
    if (phoneEmailField.startsWith('0')) {
      loginObs = this.authService.logIn(null, phoneEmailField, password);
    } else {
      loginObs = this.authService.logIn(phoneEmailField, null, password);
    }
    loginObs.pipe(finalize(() => { })).subscribe({
      next: (response) => {
        this.router.navigate(['/main']);
      },
      error: (error: Error) => {
        this.displayError(error.message);
      },
    });
  }

  private displayError(error: string) {
    this.errorMsg = error;
    setTimeout(() => {
      this.errorMsg = '';
    }, 5000);
  }

  ngOnInit(): void { }

  onGoogleLogin() {
    this.loading = true;
    this.loadingMessage = 'waiting for google signin';
    this.googleAuth
      .login()
      .then((response) => {
        console.log('sucssesfully logged', response);
        const token = response.authentication.accessToken;
        this.loginGoogle(token);

      })
      .catch((error) => {
        console.log('error', error);
      })
      .finally(() => {
        this.loading = false;
      });
  }

  onFacebokLogin() {
    this.loading = true;
    this.loadingMessage = 'waiting for facebook signin';
    this.facebookAuth
      .login()
      .then((response: any) => {
        const token = response.authResponse.accessToken;
        this.loginFacebook(token);
      })
      .catch((error) => {
        console.log(error);
        this.passErrorMessage(error.message);
        this.loading = false;
      });
  }

  private loginFacebook(token: string) {
    this.loadingMessage = 'signing in';
    this.authService.loginFaceBook(token).pipe().subscribe({
      next: () => {
        this.loadingMessage = 'log in sucssessful redirecting 5 seconds';
        setTimeout(() => {
          this.router.navigate(['/main']);
        }, 5000);
      },
      error: (error: Error) => {
        console.log(error);
        this.passErrorMessage(error.message);
        this.loading = false;
      }
    })
  }

  private loginGoogle(token: string) {
    this.loading = true;
    this.loadingMessage = 'signing in';
    this.authService.loginGoogle(token)
      .subscribe({
        next: () => {
          this.loadingMessage = 'log in sucssessful redirecting 5 seconds';
          setTimeout(() => {
            this.router.navigate(['/main']);
          }, 5000);
        },
        error: (error: Error) => {
          this.loading = false;
          this.passErrorMessage(error.message);
        }
      })
  }

  private passErrorMessage(error: string) {
    this.errorMsg = error;
    setTimeout(() => {
      this.errorMsg = '';
    }, 8000);
  }
}
