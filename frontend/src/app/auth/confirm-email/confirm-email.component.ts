import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { finalize } from 'rxjs';
import { EmailValidator } from '@angular/forms';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css',
})
export class ConfirmEmailComponent {
  token: string;
  loading: boolean;
  displayMessage: string;
  router: Router;
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.token = this.route.snapshot.paramMap.get('token') || '';
    this.loading = true;

    this.authService
      .confirmEmail(this.token)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: () => {
          this.displayMessage =
            'email confirmed you will be redirected in 5 seconds';
          setTimeout(() => {
            this.router.navigate(['/main']);
          }, 5000);
        },
        error: (error: Error) => {
          this.displayMessage = error.message;
        },
      });
  }
}
