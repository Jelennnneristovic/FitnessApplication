import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // Sve kategorije (admin vidi i neaktivne sa includeInactive=true)
  getAll(includeInactive: boolean = false): Observable<Category[]> {
    const params = new HttpParams().set('includeInactive', includeInactive.toString());
    return this.http.get<Category[]>(`${this.apiUrl}/api/categories`, { params });
  }

  getById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.apiUrl}/api/categories/${id}`);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(`${this.apiUrl}/api/categories`, request);
  }

  update(id: string, request: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.apiUrl}/api/categories/${id}`, request);
  }

  delete(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/categories/${id}`);
  }
}