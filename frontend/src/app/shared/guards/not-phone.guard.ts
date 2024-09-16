import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { inject } from '@angular/core';
import { User } from '../modules/user.module';

export const notPhoneGuard: CanActivateFn = (route, state) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const user: User = authService.currentUser;
  if (!user.phonenumber) {
    return true;
  } else {
    router.navigate(['/main']);
    return false;
  }
};
