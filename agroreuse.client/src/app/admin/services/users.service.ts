import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UsersService {
  private readonly API_URL = `${environment.apiUrl}/api/users`;

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.API_URL);
  }

  getUsersByType(type: 'Farmer' | 'Factory'): Observable<User[]> {
    return this.http.get<User[]>(`${this.API_URL}?type=${type}`);
  }

  setUserBlockStatus(userId: string, isBlocked: boolean): Observable<any> {
    return this.http.put(`${this.API_URL}/${userId}/block`, { isBlocked });
  }
}
