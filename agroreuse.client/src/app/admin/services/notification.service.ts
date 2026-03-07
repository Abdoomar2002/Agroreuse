import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import * as signalR from '@microsoft/signalr';

export interface Notification {
  id: string;
  title: string;
  message: string;
  createdAt: string;
  readAt?: string;
  orderId?: string;
  status: string;
  isRead: boolean;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly API_URL = `${environment.apiUrl}/api/notifications`;
  private hubConnection: signalR.HubConnection | null = null;

  private notificationsSubject = new BehaviorSubject<Notification[]>([]);
  public notifications$ = this.notificationsSubject.asObservable();

  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  startConnection(token: string): void {
    if (this.hubConnection) {
      return;
    }

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}/hubs/notifications`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: Notification) => {
      const current = this.notificationsSubject.value;
      this.notificationsSubject.next([notification, ...current]);
      this.unreadCountSubject.next(this.unreadCountSubject.value + 1);
    });

    this.hubConnection
      .start()
      .then(() => console.log('SignalR Connected'))
      .catch((err: Error) => console.error('SignalR Connection Error: ', err));
  }

  stopConnection(): void {
    if (this.hubConnection) {
      this.hubConnection.stop();
      this.hubConnection = null;
    }
  }

  getNotifications(): Observable<Notification[]> {
    return this.http.get<ApiResponse<Notification[]>>(this.API_URL).pipe(
      map(response => {
        this.notificationsSubject.next(response.data);
        return response.data;
      })
    );
  }

  getUnreadCount(): Observable<number> {
    return this.http.get<ApiResponse<{ unreadCount: number }>>(`${this.API_URL}/unread-count`).pipe(
      map(response => {
        this.unreadCountSubject.next(response.data.unreadCount);
        return response.data.unreadCount;
      })
    );
  }

  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}/read`, {}).pipe(
      map(response => {
        const notifications = this.notificationsSubject.value.map(n =>
          n.id === id ? { ...n, isRead: true, status: 'Read' } : n
        );
        this.notificationsSubject.next(notifications);
        this.unreadCountSubject.next(Math.max(0, this.unreadCountSubject.value - 1));
        return response;
      })
    );
  }

  markAllAsRead(): Observable<any> {
    return this.http.put(`${this.API_URL}/read-all`, {}).pipe(
      map(response => {
        const notifications = this.notificationsSubject.value.map(n => ({ ...n, isRead: true, status: 'Read' }));
        this.notificationsSubject.next(notifications);
        this.unreadCountSubject.next(0);
        return response;
      })
    );
  }
}
