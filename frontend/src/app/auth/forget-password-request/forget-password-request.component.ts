import { Component, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { NgModel } from '@angular/forms';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-forget-password-request',
  templateUrl: './forget-password-request.component.html',
  styleUrl: './forget-password-request.component.css',
})
export class ForgetPasswordRequestComponent {
  errorMsg: string;
  loading: boolean;
  @ViewChild('emailInput') emailInput: NgModel;
  constructor(private authService: AuthService, private router: Router) { }
  onUpdate() {
    this.loading = true;
    this.authService
      .forgotPassword(this.emailInput?.value)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: () => {
          this.popError('change password email sent');
        },
        error: (error: Error) => {
          this.popError(error.message);
        },
      });
  }

  private popError(error: string) {
    this.errorMsg = error;
    setTimeout(() => {
      this.errorMsg = '';
    }, 7000);
  }
}
