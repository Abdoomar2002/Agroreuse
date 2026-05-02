import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AdminLoginComponent } from './components/admin-login/admin-login.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { UsersManagementComponent } from './components/users-management/users-management.component';
import { GovernmentsComponent } from './components/governments/governments.component';
import { ContactMessagesComponent } from './components/contact-messages/contact-messages.component';
import { CategoriesComponent } from './components/categories/categories.component';
import { OrdersComponent } from './components/orders/orders.component';
import { FarmerOrdersComponent } from './components/farmer-orders/farmer-orders.component';
import { FactoryOrdersComponent } from './components/factory-orders/factory-orders.component';
import { AnalyticsComponent } from './components/analytics/analytics.component';
import { NotificationsDropdownComponent } from './components/notifications-dropdown/notifications-dropdown.component';
import { ToastComponent } from './components/toast/toast.component';
import { AdminAuthGuard } from './guards/admin-auth.guard';

const routes: Routes = [
  {
    path: 'login',
    component: AdminLoginComponent
  },
  {
    path: 'dashboard',
    component: AdminDashboardComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: 'analytics',
    component: AnalyticsComponent,
    canActivate: [AdminAuthGuard]
  },
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  }
];

@NgModule({
  declarations: [
    AdminLoginComponent,
    AdminDashboardComponent,
    UsersManagementComponent,
    GovernmentsComponent,
    ContactMessagesComponent,
    CategoriesComponent,
    OrdersComponent,
    FarmerOrdersComponent,
    FactoryOrdersComponent,
    AnalyticsComponent,
    NotificationsDropdownComponent,
    ToastComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class AdminModule { }
