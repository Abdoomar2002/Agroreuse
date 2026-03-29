import { Component, OnInit } from '@angular/core';
import { OrderService } from '../../services/order.service';
import { Order, OrderStatus, OrderStatusLabels, OrderStatusColors } from '../../models/order.models';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-farmer-orders',
  templateUrl: './farmer-orders.component.html',
  styleUrls: ['./farmer-orders.component.css']
})
export class FarmerOrdersComponent implements OnInit {
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  loading = false;
  error = '';

  // Filter state
  statusFilter: OrderStatus | 'all' = 'all';
  searchQuery = '';

  // View detail state
  selectedOrder: Order | null = null;
  showDetailModal = false;

  // Status update state
  updatingOrderId: string | null = null;

  // Status options for dropdown
  statusOptions = [
    { value: OrderStatus.Pending, label: OrderStatusLabels[OrderStatus.Pending] },
    { value: OrderStatus.Approved, label: OrderStatusLabels[OrderStatus.Approved] },
    { value: OrderStatus.Rejected, label: OrderStatusLabels[OrderStatus.Rejected] },
    { value: OrderStatus.InProgress, label: OrderStatusLabels[OrderStatus.InProgress] },
    { value: OrderStatus.Completed, label: OrderStatusLabels[OrderStatus.Completed] },
    { value: OrderStatus.Cancelled, label: OrderStatusLabels[OrderStatus.Cancelled] }
  ];

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.error = '';
    this.orderService.getFarmerOrders().subscribe({
      next: (data) => {
        this.orders = data;
        this.applyFilters();
        this.loading = false;
      },
      error: (err: Error) => {
        this.error = 'فشل تحميل طلبات المزارعين';
        this.loading = false;
        console.error('Error loading farmer orders:', err);
      }
    });
  }

  applyFilters(): void {
    let result = [...this.orders];

    // Filter by status
    if (this.statusFilter !== 'all') {
      result = result.filter(order => order.status === this.statusFilter);
    }

    // Filter by search query
    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      result = result.filter(order =>
        order.sellerName.toLowerCase().includes(query) ||
        order.categoryName.toLowerCase().includes(query) ||
        order.addressDetails.toLowerCase().includes(query)
      );
    }

    this.filteredOrders = result;
  }

  onStatusFilterChange(status: OrderStatus | 'all'): void {
    this.statusFilter = status;
    this.applyFilters();
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  getStatusLabel(status: OrderStatus): string {
    return OrderStatusLabels[status] || 'غير معروف';
  }

  getStatusColor(status: OrderStatus): string {
    return OrderStatusColors[status] || '#9e9e9e';
  }

  getImageUrl(imagePath?: string): string {
    if (!imagePath) return '';
    if (imagePath.startsWith('http')) return imagePath;
    return `${environment.apiUrl}/${imagePath}`;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('ar-EG', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  viewOrderDetails(order: Order): void {
    this.selectedOrder = order;
    this.showDetailModal = true;
  }

  closeDetailModal(): void {
    this.selectedOrder = null;
    this.showDetailModal = false;
  }

  updateOrderStatus(order: Order, newStatus: OrderStatus): void {
    if (order.status === newStatus) return;

    this.updatingOrderId = order.id;
    this.orderService.updateOrderStatus(order.id, order, newStatus).subscribe({
      next: () => {
        order.status = newStatus;
        this.updatingOrderId = null;
        this.applyFilters();
      },
      error: (err: Error) => {
        alert('فشل تحديث حالة الطلب');
        this.updatingOrderId = null;
        console.error('Error updating order status:', err);
      }
    });
  }

  deleteOrder(order: Order): void {
    if (!confirm(`هل تريد حذف هذا الطلب؟`)) return;

    this.orderService.deleteOrder(order.id).subscribe({
      next: () => {
        this.orders = this.orders.filter(o => o.id !== order.id);
        this.applyFilters();
        if (this.selectedOrder?.id === order.id) {
          this.closeDetailModal();
        }
      },
      error: (err: Error) => {
        alert('فشل حذف الطلب');
        console.error('Error deleting order:', err);
      }
    });
  }

  getOrdersCountByStatus(status: OrderStatus): number {
    return this.orders.filter(o => o.status === status).length;
  }

  getTotalOrdersCount(): number {
    return this.orders.length;
  }
}
