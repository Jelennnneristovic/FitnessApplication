import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const snackBar = inject(MatSnackBar);

  // Dodaj Bearer token
  const token = authService.getToken();
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }

  // Da li je ovo login/register zahtev (tu 401 znaci pogresni podaci, ne istekla sesija)
  const isAuthRequest = req.url.includes('/auth/login') || req.url.includes('/auth/register');

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // 401 na obicnim zahtevima = istekla sesija (ali NE na login/register)
      if (error.status === 401 && !isAuthRequest) {
        authService.logout();
        snackBar.open('Sesija je istekla. Prijavite se ponovo.', 'OK', { duration: 5000 });
        router.navigate(['/login']);
      }
      return throwError(() => error);
    })
  );
};