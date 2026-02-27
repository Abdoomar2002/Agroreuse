import { Component, OnInit } from '@angular/core';
import { AdminAnalyticsService } from '../../services/admin-analytics.service';

@Component({
  selector: 'app-analytics',
  template: `
    <div class="analytics">
      <h2>إحصائيات الطلبات</h2>
      <div *ngIf="loading">تحميل...</div>
      <div *ngIf="error" class="error">{{ error }}</div>
      <div *ngIf="stats">
        <p>إجمالي الطلبات: {{ stats.totalOrders }}</p>
        <p>إجمالي الكمية: {{ stats.totalQuantity }}</p>
        <p>متوسط الكمية: {{ stats.averageQuantity | number:'1.0-2' }}</p>

        <h3>الطلبات حسب الحالة</h3>
        <ul>
          <li *ngFor="let key of statusKeys()">{{ key }}: {{ stats.countsByStatus[key] || 0 }}</li>
        </ul>

        <h3>الطلبات خلال الأشهر الأخيرة</h3>
        <ul>
          <li *ngFor="let m of stats.ordersByMonth">{{ m.month }}: {{ m.count }}</li>
        </ul>
      </div>
    </div>
  `,
  styles: [`.analytics { padding: 16px; } .error { color: red; }`]
})
export class AnalyticsComponent implements OnInit {
  stats: any = null;
  loading = false;
  error = '';

  constructor(private analyticsService: AdminAnalyticsService) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading = true;
    this.error = '';
    this.analyticsService.getOrdersStatistics().subscribe({
      next: (res) => {
        this.stats = res?.data || res;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل تحميل الإحصائيات';
        this.loading = false;
        console.error(err);
      }
    });
  }

  statusKeys(): string[] {
    return this.stats ? Object.keys(this.stats.countsByStatus || {}) : [];
  }
}
