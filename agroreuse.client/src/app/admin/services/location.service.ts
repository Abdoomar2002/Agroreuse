import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Government, City } from '../models/location.models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LocationService {
  private readonly GOV_URL = `${environment.apiUrl}/api/governments`;
  private readonly CITY_URL = `${environment.apiUrl}/api/cities`;

  constructor(private http: HttpClient) {}

  // Governments
  getGovernments(includeCities: boolean = false): Observable<Government[]> {
    return this.http.get<Government[]>(`${this.GOV_URL}?includeCities=${includeCities}`);
  }

  getGovernment(id: string): Observable<Government> {
    return this.http.get<Government>(`${this.GOV_URL}/${id}?includeCities=true`);
  }

  createGovernment(name: string): Observable<Government> {
    return this.http.post<Government>(this.GOV_URL, { name });
  }

  updateGovernment(id: string, name: string): Observable<any> {
    return this.http.put(`${this.GOV_URL}/${id}`, { name });
  }

  deleteGovernment(id: string): Observable<any> {
    return this.http.delete(`${this.GOV_URL}/${id}`);
  }

  // Cities
  getCities(governmentId?: string): Observable<City[]> {
    const url = governmentId ? `${this.CITY_URL}?governmentId=${governmentId}` : this.CITY_URL;
    return this.http.get<City[]>(url);
  }

  createCity(name: string, governmentId: string): Observable<City> {
    return this.http.post<City>(this.CITY_URL, { name, governmentId });
  }

  updateCity(id: string, name: string): Observable<any> {
    return this.http.put(`${this.CITY_URL}/${id}`, { name });
  }

  deleteCity(id: string): Observable<any> {
    return this.http.delete(`${this.CITY_URL}/${id}`);
  }
}
