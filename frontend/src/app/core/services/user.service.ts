import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserListItem, UserStatus } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Lista klijenata sa opcionim filterom
  getClients(keyword?: string, status?: UserStatus): Observable<UserListItem[]> {
    let params = new HttpParams();
    if (keyword) params = params.set('keyword', keyword);
    if (status !== undefined && status !== null) params = params.set('status', status.toString());

    return this.http.get<UserListItem[]>(`${this.apiUrl}/api/users/clients`, { params });
  }

  // Lista trenera sa opcionim filterom
  getTrainers(keyword?: string, status?: UserStatus): Observable<UserListItem[]> {
    let params = new HttpParams();
    if (keyword) params = params.set('keyword', keyword);
    if (status !== undefined && status !== null) params = params.set('status', status.toString());

    return this.http.get<UserListItem[]>(`${this.apiUrl}/api/users/trainers`, { params });
  }

  // Aktiviraj korisnika
  activate(userId: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/api/users/${userId}/activate`, {});
  }

  // Deaktiviraj korisnika
  deactivate(userId: string): Observable<any> {
    return this.http.patch(`${this.apiUrl}/api/users/${userId}/deactivate`, {});
  }
}