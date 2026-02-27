import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AdminAuthService } from '../../services/admin-auth.service';
import { AdminUser } from '../../models/auth.models';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  currentUser: AdminUser | null = null;
  currentView: 'users' | 'governments' | 'categories' | 'contact' | 'orders' | 'analytics' = 'users';

  constructor(
    private authService: AdminAuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  navigateTo(view: 'users' | 'governments' | 'categories' | 'contact' | 'orders' | 'analytics'): void {
    this.currentView = view;
  }

  logout(): void {
    if (confirm('هل تريد تسجيل الخروج؟')) {
      this.authService.logout();
      this.router.navigate(['/admin/login']);
    }
  }
}
