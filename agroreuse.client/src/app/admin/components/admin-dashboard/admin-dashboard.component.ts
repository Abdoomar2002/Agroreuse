import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AdminAuthService } from '../../services/admin-auth.service';
import { NotificationService } from '../../services/notification.service';
import { AdminUser } from '../../models/auth.models';

type ViewType = 'users' | 'governments' | 'categories' | 'contact' | 'orders' | 'farmer-orders' | 'factory-orders' | 'analytics';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  currentUser: AdminUser | null = null;
  currentView: ViewType = 'users';
  sidebarOpen = false;

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

  navigateTo(view: ViewType): void {
    this.currentView = view;
    this.sidebarOpen = false;
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebar(): void {
    this.sidebarOpen = false;
  }

  logout(): void {
    if (confirm('هل تريد تسجيل الخروج؟')) {
      this.notificationService.stopConnection();
      this.authService.logout();
      this.router.navigate(['/admin/login']);
    }
  }
}
