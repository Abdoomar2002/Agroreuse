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
    CategoriesComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class AdminModule { }
