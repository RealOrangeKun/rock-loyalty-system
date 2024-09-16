import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../auth/auth.service';
import { inject } from '@angular/core';

export const phoneGuardGuard: CanActivateFn = (route, state) => {
  const authService: AuthService = inject(AuthService);
  const router: Router = inject(Router);
  const user = authService.user.getValue();
  if (user.phonenumber) {
    return true;
  } else {
    router.navigate(['/phone']);
    return false;
  }
};
