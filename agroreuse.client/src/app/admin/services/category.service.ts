import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category } from '../models/category.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly API_URL = `${environment.apiUrl}/api/categories`;

  constructor(private http: HttpClient) {}

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.API_URL);
  }

  getCategory(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.API_URL}/${id}`);
  }

  createCategory(name: string, image?: File): Observable<Category> {
    const formData = new FormData();
    if (image) {
      formData.append('image', image);
    }
    return this.http.post<Category>(`${this.API_URL}?name=${encodeURIComponent(name)}`, formData);
  }

  updateCategory(id: string, name?: string, image?: File): Observable<any> {
    const formData = new FormData();
    if (name) {
      formData.append('name', name);
    }
    if (image) {
      formData.append('image', image);
    }
    return this.http.put(`${this.API_URL}/${id}`, formData);
  }

  deleteCategory(id: string): Observable<any> {
    return this.http.delete(`${this.API_URL}/${id}`);
  }
}
