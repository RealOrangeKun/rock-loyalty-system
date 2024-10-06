import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../auth.service';
import { finalize } from 'rxjs';
import { EmailValidator } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css',
})
export class ConfirmEmailComponent {
  token: string;
  loading: boolean;
  displayMessage: string;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private toastrService: ToastrService
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
          this.toastrService.success(
            'redirected in 5 seconds',
            'email confirmed'
          );
          setTimeout(() => {
            this.router.navigate([this.authService.restaurantId, 'main']);
          }, 5000);
        },
        error: (error: Error) => {
          this.toastrService.error(
            `couldn't confirm your email please try again`
          );
          this.router.navigate([
            this.authService.restaurantId,
            'auth',
            'register',
          ]);
        },
      });
  }
}
