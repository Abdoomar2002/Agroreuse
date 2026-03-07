import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminAnalyticsService {
  private readonly API_URL = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) {}

  getOrdersStatistics(): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.API_URL}/stats`).pipe(
      map(response => response)
    );
  }

  getMyOrdersStatistics(): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.API_URL}/my-stats`).pipe(
      map(response => response)
    );
  }
}
