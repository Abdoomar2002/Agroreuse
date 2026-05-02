import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactMessage } from '../models/contact.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  private readonly API_URL = `${environment.apiUrl}/api/contactus`;

  constructor(private http: HttpClient) {}

  getAllMessages(isRead?: boolean): Observable<ContactMessage[]> {
    let url = this.API_URL;
    if (isRead !== undefined) {
      url += `?isRead=${isRead}`;
    }
    return this.http.get<ContactMessage[]>(url);
  }

  getMessage(id: string): Observable<ContactMessage> {
    return this.http.get<ContactMessage>(`${this.API_URL}/${id}`);
  }

  markAsRead(id: string): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}/mark-read`, {});
  }

  respondToMessage(id: string, response: string): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}/respond`, JSON.stringify(response), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  deleteMessage(id: string): Observable<any> {
    return this.http.delete(`${this.API_URL}/${id}`);
  }
}
