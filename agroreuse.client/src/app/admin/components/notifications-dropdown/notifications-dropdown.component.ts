import { Component, OnInit, OnDestroy } from '@angular/core';
import { NotificationService, Notification } from '../../services/notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notifications-dropdown',
  template: `
    <div class="notifications-wrapper">
      <button class="notification-bell" (click)="toggleDropdown()">
        <svg viewBox="0 0 24 24" fill="none">
          <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
          <path d="M13.73 21a2 2 0 0 1-3.46 0" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
        </svg>
        <span class="badge" *ngIf="unreadCount > 0">{{ unreadCount > 9 ? '9+' : unreadCount }}</span>
      </button>

      <div class="dropdown" *ngIf="isOpen">
        <div class="dropdown-header">
          <h3>الإشعارات</h3>
          <button *ngIf="unreadCount > 0" class="mark-all-btn" (click)="markAllRead()">تحديد الكل كمقروء</button>
        </div>
        <div class="dropdown-body">
          <div *ngIf="notifications.length === 0" class="empty-state">
            لا توجد إشعارات
          </div>
          <div *ngFor="let notification of notifications" 
               class="notification-item" 
               [class.unread]="!notification.isRead"
               (click)="markAsRead(notification)">
            <div class="notification-content">
              <p class="notification-title">{{ notification.title }}</p>
              <p class="notification-message">{{ notification.message }}</p>
              <span class="notification-time">{{ formatDate(notification.createdAt) }}</span>
            </div>
            <span class="unread-dot" *ngIf="!notification.isRead"></span>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notifications-wrapper { position: relative; }
    .notification-bell {
      background: none; border: none; cursor: pointer; position: relative;
      padding: 8px; border-radius: 8px; transition: background 0.2s;
    }
    .notification-bell:hover { background: rgba(0,0,0,0.05); }
    .notification-bell svg { width: 24px; height: 24px; color: #5f6368; }
    .badge {
      position: absolute; top: 2px; right: 2px;
      background: #c62828; color: #fff; font-size: 10px; font-weight: 600;
      min-width: 18px; height: 18px; border-radius: 9px;
      display: flex; align-items: center; justify-content: center;
    }
    .dropdown {
      position: absolute; top: 100%; left: 0; width: 320px;
      background: #fff; border-radius: 12px; box-shadow: 0 4px 20px rgba(0,0,0,0.15);
      z-index: 1000; overflow: hidden; margin-top: 8px;
    }
    .dropdown-header {
      display: flex; justify-content: space-between; align-items: center;
      padding: 16px; border-bottom: 1px solid #f0f0f0;
    }
    .dropdown-header h3 { margin: 0; font-size: 16px; color: #1f1f1f; }
    .mark-all-btn {
      background: none; border: none; color: #1E4D2B; font-size: 12px;
      cursor: pointer; font-weight: 600;
    }
    .dropdown-body { max-height: 400px; overflow-y: auto; }
    .empty-state { padding: 40px 16px; text-align: center; color: #9e9e9e; }
    .notification-item {
      display: flex; padding: 12px 16px; border-bottom: 1px solid #f5f5f5;
      cursor: pointer; transition: background 0.2s; position: relative;
    }
    .notification-item:hover { background: #f9faf8; }
    .notification-item.unread { background: #f0f7f1; }
    .notification-content { flex: 1; }
    .notification-title { margin: 0 0 4px; font-weight: 600; font-size: 14px; color: #1f1f1f; }
    .notification-message { margin: 0 0 4px; font-size: 13px; color: #5f6368; }
    .notification-time { font-size: 11px; color: #9e9e9e; }
    .unread-dot {
      width: 8px; height: 8px; background: #1E4D2B; border-radius: 50%;
      margin-right: 8px; flex-shrink: 0; align-self: center;
    }
  `]
})
export class NotificationsDropdownComponent implements OnInit, OnDestroy {
  notifications: Notification[] = [];
  unreadCount = 0;
  isOpen = false;
  private subscriptions: Subscription[] = [];

  constructor(private notificationService: NotificationService) {}

  ngOnInit(): void {
    this.subscriptions.push(
      this.notificationService.notifications$.subscribe(n => this.notifications = n),
      this.notificationService.unreadCount$.subscribe(c => this.unreadCount = c)
    );
    this.loadNotifications();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  loadNotifications(): void {
    this.notificationService.getNotifications().subscribe();
    this.notificationService.getUnreadCount().subscribe();
  }

  toggleDropdown(): void {
    this.isOpen = !this.isOpen;
  }

  markAsRead(notification: Notification): void {
    if (!notification.isRead) {
      this.notificationService.markAsRead(notification.id).subscribe();
    }
  }

  markAllRead(): void {
    this.notificationService.markAllAsRead().subscribe();
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (minutes < 1) return 'الآن';
    if (minutes < 60) return `منذ ${minutes} دقيقة`;
    if (hours < 24) return `منذ ${hours} ساعة`;
    if (days < 7) return `منذ ${days} يوم`;
    return date.toLocaleDateString('ar-EG');
  }
}
