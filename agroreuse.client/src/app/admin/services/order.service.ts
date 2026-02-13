import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { Order, OrderStatus, UpdateOrderRequest } from '../models/order.models';
import { environment } from '../../../environments/environment';

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private readonly API_URL = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) {}

  getAllOrders(): Observable<Order[]> {
    return this.http.get<ApiResponse<Order[]>>(this.API_URL).pipe(
      map(response => response.data)
    );
  }

  getOrderById(id: string): Observable<Order> {
    return this.http.get<ApiResponse<Order>>(`${this.API_URL}/${id}`).pipe(
      map(response => response.data)
    );
  }

  getOrdersBySeller(sellerId: string): Observable<Order[]> {
    return this.http.get<ApiResponse<Order[]>>(`${this.API_URL}/seller/${sellerId}`).pipe(
      map(response => response.data)
    );
  }

  updateOrderStatus(id: string, order: Order, newStatus: OrderStatus): Observable<any> {
    const updateRequest: UpdateOrderRequest = {
      addressId: order.addressId,
      categoryId: order.categoryId,
      quantity: order.quantity,
      numberOfDays: order.numberOfDays,
      status: newStatus
    };
    return this.http.put(`${this.API_URL}/${id}`, updateRequest);
  }

  updateOrder(id: string, request: UpdateOrderRequest): Observable<any> {
    return this.http.put(`${this.API_URL}/${id}`, request);
  }

  deleteOrder(id: string): Observable<any> {
    return this.http.delete(`${this.API_URL}/${id}`);
  }

  getOrderImages(orderId: string): Observable<{ id: string; imagePath: string }[]> {
    return this.http.get<ApiResponse<{ id: string; imagePath: string }[]>>(`${this.API_URL}/${orderId}/images`).pipe(
      map(response => response.data)
    );
  }

  uploadOrderImages(orderId: string, images: File[]): Observable<any> {
    const formData = new FormData();
    images.forEach(image => {
      formData.append('images', image);
    });
    return this.http.post(`${this.API_URL}/${orderId}/images?orderId=${orderId}`, formData);
  }

  deleteOrderImage(orderId: string, imageId: string): Observable<any> {
    return this.http.delete(`${this.API_URL}/${orderId}/images/${imageId}`);
  }
}
