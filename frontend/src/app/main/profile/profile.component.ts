import {
  AfterViewInit,
  Component,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { AuthService } from '../../auth/auth.service';
import { NgForm } from '@angular/forms';
import { User } from '../../shared/modules/user.module';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
  @ViewChild('myForm') myForm: NgForm;
  name: string;
  email: string;
  phoneNumber: string;
  constructor(private authService: AuthService) {}

  ngOnInit() {
    const user: User = this.authService.currentUser;
    this.name = user.name;
    this.email = user.email;
    this.phoneNumber = user.phonenumber;
  }

  onSubmit() {
    console.log(this.myForm.value);
  }

  onLogOut() {
    this.authService.LogOut();
  }
}
