import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { NgModel } from '@angular/forms';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-forget-password',
  templateUrl: './forget-password.component.html',
  styleUrl: './forget-password.component.css',
})
export class ForgetPasswordComponent implements OnInit {
  token: string = '';
  loading: boolean;
  errorMsg: string;
  @ViewChild('passwordInput') passwordField: NgModel;
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.paramMap.get('token') || '';
  }

  onUpdate() {
    this.loading = true;
    this.authService
      .UpdatePassword(this.token, this.passwordField.value)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/main']);
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
