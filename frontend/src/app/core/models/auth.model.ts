import { User, UserRole, UserGender } from './user.model';

// Sta saljemo na POST /api/auth/login
export interface LoginRequest {
  username: string;
  password: string;
}

// Sta saljemo na POST /api/auth/register
export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  dateOfBirth: string;   // format "YYYY-MM-DD"
  gender: UserGender;
  location: string;
  role: UserRole;
}

// Sta backend VRACA na login/register (Auth objekat)
export interface AuthResponse {
  token: string;
  user: User;
}