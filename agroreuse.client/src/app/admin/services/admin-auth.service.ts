import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, AdminUser } from '../models/auth.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AdminAuthService {
  private readonly API_URL = `${environment.apiUrl}/api/auth`;
  private readonly TOKEN_KEY = 'adminToken';
  private readonly USER_KEY = 'adminUser';

  private currentUserSubject: BehaviorSubject<AdminUser | null>;
  public currentUser: Observable<AdminUser | null>;

  constructor(private http: HttpClient) {
    const storedUser = this.getStoredUser();
    this.currentUserSubject = new BehaviorSubject<AdminUser | null>(storedUser);
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): AdminUser | null {
    return this.currentUserSubject.value;
  }

  login(
    email: string,
    password: string,
    rememberMe: boolean = false,
  ): Observable<LoginResponse> {
    const request = {
      email: email,
      password: password,
      userType: 2,
    };

    return this.http.post<any>(`${this.API_URL}/login`, request).pipe(
      tap((response) => {
        // Store token
        const storage = rememberMe ? localStorage : sessionStorage;
        storage.setItem(this.TOKEN_KEY, response.token);

        const user: AdminUser = {
          id: response.id || '',
          email: response.email,
          firstName:
            response.fullName?.split(' ')[0] || response.fullName || '',
          lastName: response.fullName?.split(' ').slice(1).join(' ') || '',
          userType: 'Admin',
        };

        storage.setItem(this.USER_KEY, JSON.stringify(user));
        this.currentUserSubject.next(user);
      }),
    );
  }

  logout(): void {
    // Clear from both storages
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);

    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return (
      localStorage.getItem(this.TOKEN_KEY) ||
      sessionStorage.getItem(this.TOKEN_KEY)
    );
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    const user = this.currentUserValue;
    return !!token && !!user && user.userType === 'Admin';
  }

  private getStoredUser(): AdminUser | null {
    const userStr =
      localStorage.getItem(this.USER_KEY) ||
      sessionStorage.getItem(this.USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  }
}
