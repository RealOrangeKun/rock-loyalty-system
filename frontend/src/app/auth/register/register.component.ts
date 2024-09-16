import { Component, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { AuthService } from '../auth.service';
import { finalize } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  @ViewChild('registerForm') form: NgForm;
  errorMsg: string = '';
  isLoading: boolean = false;
  constructor(private authService: AuthService) {}
  onSubmit() {}
}
