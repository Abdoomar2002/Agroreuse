import { Component, OnInit } from '@angular/core';
import { AdminAnalyticsService } from '../../services/admin-analytics.service';
import { OrderService } from '../../services/order.service';
import { Order, OrderStatus, OrderStatusLabels, OrderStatusColors } from '../../models/order.models';

interface MonthlyData {
  month: string;
  count: number;
}

interface StatsData {
  totalOrders: number;
  countsByStatus: { [key: string]: number };
  totalQuantity: number;
  averageQuantity: number;
  ordersByMonth: MonthlyData[];
}

@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.css']
})
export class AnalyticsComponent implements OnInit {
  stats: StatsData | null = null;
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  loading = false;
  error = '';

  // Filters
  searchQuery = '';
  selectedStatus: OrderStatus | 'all' = 'all';
  selectedCategory = 'all';
  dateFrom = '';
  dateTo = '';

  // Categories extracted from orders
  categories: string[] = [];

  // Status options
  statusOptions = [
    { value: OrderStatus.Pending, label: OrderStatusLabels[OrderStatus.Pending] },
    { value: OrderStatus.Approved, label: OrderStatusLabels[OrderStatus.Approved] },
    { value: OrderStatus.Rejected, label: OrderStatusLabels[OrderStatus.Rejected] },
 ];

  constructor(
    private analyticsService: AdminAnalyticsService,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading = true;
    this.error = '';

    // Load both stats and orders
    this.analyticsService.getOrdersStatistics().subscribe({
      next: (res) => {
        this.stats = res?.data || res;
      },
      error: (err: Error) => {
        console.error('Stats error:', err);
      }
    });

    this.orderService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.extractCategories();
        this.applyFilters();
        this.loading = false;
      },
      error: (err: Error) => {
        this.error = 'فشل تحميل البيانات';
        this.loading = false;
        console.error(err);
      }
    });
  }

  extractCategories(): void {
    const cats = new Set(this.orders.map(o => o.categoryName));
    this.categories = Array.from(cats).filter(c => c);
  }

  applyFilters(): void {
    let result = [...this.orders];

    // Filter by status
    if (this.selectedStatus !== 'all') {
      result = result.filter(o => o.status === this.selectedStatus);
    }

    // Filter by category
    if (this.selectedCategory !== 'all') {
      result = result.filter(o => o.categoryName === this.selectedCategory);
    }

    // Filter by search
    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      result = result.filter(o =>
        o.sellerName.toLowerCase().includes(query) ||
        o.categoryName.toLowerCase().includes(query) ||
        o.addressDetails.toLowerCase().includes(query)
      );
    }

    // Filter by date range
    if (this.dateFrom) {
      const from = new Date(this.dateFrom);
      result = result.filter(o => new Date(o.createdAt) >= from);
    }
    if (this.dateTo) {
      const to = new Date(this.dateTo);
      to.setHours(23, 59, 59);
      result = result.filter(o => new Date(o.createdAt) <= to);
    }

    this.filteredOrders = result;
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.selectedStatus = 'all';
    this.selectedCategory = 'all';
    this.dateFrom = '';
    this.dateTo = '';
    this.applyFilters();
  }

  getStatusLabel(status: OrderStatus): string {
    return OrderStatusLabels[status] || 'غير معروف';
  }

  getStatusColor(status: OrderStatus): string {
    return OrderStatusColors[status] || '#9e9e9e';
  }

  getStatusCount(statusName: string): number {
    return this.stats?.countsByStatus?.[statusName] || 0;
  }

  getTotalByStatus(status: OrderStatus): number {
    return this.orders.filter(o => o.status === status).length;
  }

  getFilteredTotalQuantity(): number {
    return this.filteredOrders.reduce((sum, o) => sum + o.quantity, 0);
  }

  getFilteredAverageQuantity(): number {
    if (this.filteredOrders.length === 0) return 0;
    return this.getFilteredTotalQuantity() / this.filteredOrders.length;
  }

  formatMonth(monthStr: string): string {
    const [year, month] = monthStr.split('-');
    const date = new Date(parseInt(year), parseInt(month) - 1);
    return date.toLocaleDateString('ar-EG', { month: 'short', year: 'numeric' });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('ar-EG', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  getMaxMonthCount(): number {
    if (!this.stats?.ordersByMonth) return 1;
    return Math.max(...this.stats.ordersByMonth.map(m => m.count), 1);
  }

  getBarHeight(count: number): number {
    return (count / this.getMaxMonthCount()) * 100;
  }
}
