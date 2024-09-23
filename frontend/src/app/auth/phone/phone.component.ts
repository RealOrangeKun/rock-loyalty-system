import { Component, ViewChild } from '@angular/core';
import { AuthService } from '../auth.service';
import { NgForm } from '@angular/forms';
import { User } from '../../shared/modules/user.module';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-phone',
  templateUrl: './phone.component.html',
  styleUrl: './phone.component.css',
})
export class PhoneComponent {
  @ViewChild('loginForm') form: NgForm;
  constructor(
    private authService: AuthService,
    private router: Router,
    private toastrService: ToastrService
  ) {}
  loading: boolean = false;
  onSubmit() {
    const phone: string = this.form.value.phone;
    const user: User = this.authService.currentUser;
    user.phonenumber = phone;
    this.loading = true;
    this.authService
      .updateUserInfo(user)
      .pipe(
        finalize(() => {
          this.loading = false;
        })
      )
      .subscribe({
        next: (response) => {
          this.toastrService.success('redirecting in 5 seconds');
          setTimeout(() => {
            this.router.navigate(['/main']);
          }, 5000);
        },
        error: (error) => {
          this.toastrService.error(error.message);
        },
      });
  }
}
