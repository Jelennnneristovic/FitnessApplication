import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserRole } from '../models/user.model';

// Guard: mora biti ulogovan
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  }

  // Nije ulogovan - na login
  router.navigate(['/login']);
  return false;
};

// Guard: mora imati odredjenu rolu
// Koristi se sa 'data: { role: UserRole.Admin }' u ruti
export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Prvo - mora biti ulogovan
  if (!authService.isLoggedIn()) {
    router.navigate(['/login']);
    return false;
  }

  // Procitaj koja rola je potrebna za ovu rutu
  const requiredRole = route.data['role'] as UserRole;
  const userRole = authService.getUserRole();

  if (userRole === requiredRole) {
    return true;
  }

  // Pogresna rola - vrati na njegov dashboard
  router.navigate(['/']);
  return false;
};