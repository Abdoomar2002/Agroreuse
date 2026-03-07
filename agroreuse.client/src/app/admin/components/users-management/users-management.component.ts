import { Component, OnInit } from '@angular/core';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user.models';

@Component({
  selector: 'app-users-management',
  templateUrl: './users-management.component.html',
  styleUrls: ['./users-management.component.css']
})
export class UsersManagementComponent implements OnInit {
  allUsers: User[] = [];
  filteredUsers: User[] = [];
  loading = false;
  error = '';

  // Filters
  searchQuery = '';
  selectedType: 'all' | 0 | 1 = 'all'; // 0=Farmer, 1=Factory
  selectedStatus: 'all' | 'active' | 'locked' = 'all';

  // Block status update
  updatingUserId: string | null = null;

  constructor(private usersService: UsersService) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.error = '';

    this.usersService.getAllUsers().subscribe({
      next: (users) => {
        this.allUsers = users;
        this.applyFilters();
        this.loading = false;
      },
      error: (err) => {
        this.error = 'فشل تحميل المستخدمين. حاول مرة أخرى.';
        this.loading = false;
        console.error(err);
      }
    });
  }

  applyFilters(): void {
    let result = [...this.allUsers];

    // Filter by type
    if (this.selectedType !== 'all') {
      result = result.filter(u => u.type === this.selectedType);
    }

    // Filter by status
    if (this.selectedStatus === 'active') {
      result = result.filter(u => !u.isLocked);
    } else if (this.selectedStatus === 'locked') {
      result = result.filter(u => u.isLocked);
    }

    // Filter by search
    if (this.searchQuery.trim()) {
      const query = this.searchQuery.toLowerCase();
      result = result.filter(u =>
        u.fullName.toLowerCase().includes(query) ||
        u.email.toLowerCase().includes(query) ||
        (u.phoneNumber && u.phoneNumber.includes(query)) ||
        (u.address && u.address.toLowerCase().includes(query))
      );
    }

    this.filteredUsers = result;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onTypeChange(type: 'all' | 0 | 1): void {
    this.selectedType = type;
    this.applyFilters();
  }

  onStatusChange(status: 'all' | 'active' | 'locked'): void {
    this.selectedStatus = status;
    this.applyFilters();
  }

  toggleBlockStatus(user: User): void {
    const newStatus = !user.isLocked;
    const action = newStatus ? 'حظر' : 'إلغاء حظر';

    if (!confirm(`هل تريد ${action} المستخدم "${user.fullName}"؟`)) {
      return;
    }

    this.updatingUserId = user.id;
    this.usersService.setUserBlockStatus(user.id, newStatus).subscribe({
      next: () => {
        user.isLocked = newStatus;
        this.updatingUserId = null;
        this.applyFilters();
      },
      error: (err) => {
        alert(`فشل ${action} المستخدم`);
        this.updatingUserId = null;
        console.error(err);
      }
    });
  }

  get farmersCount(): number {
    return this.allUsers.filter(u => u.type === 0).length;
  }

  get factoriesCount(): number {
    return this.allUsers.filter(u => u.type === 1).length;
  }

  getUserTypeLabel(type: number): string {
    const labels: { [key: number]: string } = {
      0: 'مزارع',
      1: 'مصنع',
      2: 'إدارة'
    };
    return labels[type] || 'غير معروف';
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('ar-EG', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }
}
