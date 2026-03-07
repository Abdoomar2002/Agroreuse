import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AdminAuthService } from '../../services/admin-auth.service';
import { NotificationService } from '../../services/notification.service';
import { AdminUser } from '../../models/auth.models';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  currentUser: AdminUser | null = null;
  currentView: 'users' | 'governments' | 'categories' | 'contact' | 'orders' | 'analytics' = 'users';

  constructor(
    private authService: AdminAuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser.subscribe(user => {
      this.currentUser = user;
    });

    // Start SignalR connection for real-time notifications
    const token = this.authService.getToken();
    if (token) {
      this.notificationService.startConnection(token);
    }
  }

  ngOnDestroy(): void {
    this.notificationService.stopConnection();
  }

  navigateTo(view: 'users' | 'governments' | 'categories' | 'contact' | 'orders' | 'analytics'): void {
    this.currentView = view;
  }

  logout(): void {
    if (confirm('هل تريد تسجيل الخروج؟')) {
      this.notificationService.stopConnection();
      this.authService.logout();
      this.router.navigate(['/admin/login']);
    }
  }
}
