// Enumi - moraju da odgovaraju backend enumima (po VREDNOSTI)
export enum UserRole {
  Admin = 0,
  Trainer = 1,
  Client = 2
}

export enum UserStatus {
  Active = 0,
  InActive = 1,
  PendingApproval = 2
}

export enum UserGender {
  Male = 0,
  Female = 1
}

// Korisnik kako ga backend vraca u AuthResponse
export interface User {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  status: UserStatus;
}

// Korisnik u listi (admin pregled) - sto vraca /api/users/clients i /trainers
export interface UserListItem {
  id: string;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: UserRole;
  status: UserStatus;
  profileImageUrl?: string;
}