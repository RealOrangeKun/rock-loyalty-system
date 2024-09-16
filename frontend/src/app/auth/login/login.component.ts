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
    private router: Router,
    private ngZone: NgZone
  ) {}
  onSubmit() {
    const phoneEmailField: string = this.form.value.phone;
    const password: string = this.form.value.password;
    let loginObs: Observable<UserInterface>;
    if (phoneEmailField.startsWith('0')) {
      loginObs = this.authService.logIn(null, phoneEmailField, password);
    } else {
      loginObs = this.authService.logIn(phoneEmailField, null, password);
    }
    loginObs.pipe(finalize(() => {})).subscribe({
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

  ngOnInit(): void {}

  onFacebokLogin() {
    this.loading = true;
    this.loadingMessage = 'waiting for facebook signin';
    this.facebookAuth
      .login()
      .then((response: any) => {
        console.log('Facebook login successful', response);
      })
      .catch((error) => {
        console.log('Facebook login failed', error);
      })
      .finally(() => {
        this.loading = false;
      });
  }

  onGoogleLogin() {
    console.log('start');
    this.loading = true;
    this.loadingMessage = 'waiting for google signin';
    this.googleAuth
      .login()
      .then((response) => {
        console.log('sucssesfully logged', response);
      })
      .catch((error) => {
        console.log('error', error);
      })
      .finally(() => {
        this.loading = false;
      });
  }

  private passErrorMessage(error: string) {
    this.errorMsg = error;
    setTimeout(() => {
      this.errorMsg = '';
    }, 8000);
  }
}
