import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, AuthResponse } from '../models/auth.model';
import { User, UserRole } from '../models/user.model';

@Injectable({
  providedIn: 'root'   // servis je dostupan svuda u aplikaciji
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  // Signal koji drzi trenutnog korisnika (reaktivno - UI se sam azurira)
  private currentUserSignal = signal<User | null>(this.loadUserFromStorage());

  // Javni read-only pristup signalu
  currentUser = this.currentUserSignal.asReadonly();

  // Computed: da li je korisnik ulogovan
  isLoggedIn = computed(() => this.currentUserSignal() !== null);

  constructor(private http: HttpClient) {}

  // === LOGIN ===
  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/api/auth/login`, request)
      .pipe(
        tap(response => this.handleAuthSuccess(response))
      );
  }

  // === REGISTER ===
  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/api/auth/register`, request)
      .pipe(
        tap(response => this.handleAuthSuccess(response))
      );
  }

  // === LOGOUT ===
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSignal.set(null);
  }

  // === TOKEN GETTER (interceptor ce ovo koristiti) ===
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  // === ROLE HELPERS ===
  getUserRole(): UserRole | null {
    return this.currentUserSignal()?.role ?? null;
  }

  isAdmin(): boolean {
    return this.currentUserSignal()?.role === UserRole.Admin;
  }

  isTrainer(): boolean {
    return this.currentUserSignal()?.role === UserRole.Trainer;
  }

  isClient(): boolean {
    return this.currentUserSignal()?.role === UserRole.Client;
  }

  // === PRIVATNE POMOCNE METODE ===
  private handleAuthSuccess(response: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response.user));
    this.currentUserSignal.set(response.user);
  }

  private loadUserFromStorage(): User | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    return userJson ? JSON.parse(userJson) : null;
  }
}