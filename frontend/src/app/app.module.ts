import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MainComponent } from './main/main.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { AuthComponent } from './auth/auth.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NotFoundComponent } from './shared/components/not-found/not-found.component';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './shared/interceptor/auth.interceptor';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AlertComponent } from './shared/components/alert/alert.component';
import { CommonModule } from '@angular/common';
import { PhoneComponent } from './auth/phone/phone.component';
import { PopMessageComponent } from './shared/components/pop-message/pop-message.component';
import { ConfirmEmailComponent } from './auth/confirm-email/confirm-email.component';
import { ForgetPasswordComponent } from './auth/forget-password/forget-password.component';
import { ForgetPasswordRequestComponent } from './auth/forget-password-request/forget-password-request.component';
import { ExchangePointsComponent } from './main/exchange-points/exchange-points.component';
import { PointsComponent } from './main/points/points.component';
import { VouchersComponent } from './main/vouchers/vouchers.component';
import { ProfileComponent } from './main/profile/profile.component';
import { NavigatorComponent } from './main/navigator/navigator.component';

@NgModule({
  declarations: [
    AppComponent,
    MainComponent,
    LoginComponent,
    RegisterComponent,
    AuthComponent,
    NotFoundComponent,
    AlertComponent,
    PhoneComponent,
    PopMessageComponent,
    ConfirmEmailComponent,
    ForgetPasswordComponent,
    ForgetPasswordRequestComponent,
    ExchangePointsComponent,
    PointsComponent,
    VouchersComponent,
    ProfileComponent,
    NavigatorComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    CommonModule,
    BrowserAnimationsModule,
    BrowserAnimationsModule,
    BsDropdownModule.forRoot(),
  ],
  providers: [provideHttpClient(withInterceptors([authInterceptor]))],
  bootstrap: [AppComponent],
})
export class AppModule {}
