export interface Order {
  id: string;
  sellerId: string;
  sellerName: string;
  addressId: string;
  addressDetails: string;
  categoryId: string;
  categoryName: string;
  description?: string;
  quantity: number;
  numberOfDays: string;
  status: OrderStatus;
  createdAt: string;
  imagePaths: string[];
}

export enum OrderStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5
}

export const OrderStatusLabels: { [key in OrderStatus]: string } = {
  [OrderStatus.Pending]: 'قيد الانتظار',
  [OrderStatus.Approved]: 'موافق عليه',
  [OrderStatus.Rejected]: 'مرفوض',
  [OrderStatus.InProgress]: 'قيد التنفيذ',
  [OrderStatus.Completed]: 'مكتمل',
  [OrderStatus.Cancelled]: 'ملغي'
};

export const OrderStatusColors: { [key in OrderStatus]: string } = {
  [OrderStatus.Pending]: '#ff9800',
  [OrderStatus.Approved]: '#4caf50',
  [OrderStatus.Rejected]: '#f44336',
  [OrderStatus.InProgress]: '#2196f3',
  [OrderStatus.Completed]: '#1E4D2B',
  [OrderStatus.Cancelled]: '#9e9e9e'
};

export interface CreateOrderRequest {
  address: AddressRequest;
  categoryId: string;
  quantity: number;
  numberOfDays: string;
  description?: string;
  imagePaths?: string[];
}

export interface AddressRequest {
  governmentId: string;
  cityId: string;
  details: string;
}

export interface UpdateOrderRequest {
  addressId: string;
  categoryId: string;
  quantity: number;
  numberOfDays: string;
  status: OrderStatus;
  description?: string;
}

export interface OrderResponse {
  success: boolean;
  data: Order | Order[];
  message: string;
}
