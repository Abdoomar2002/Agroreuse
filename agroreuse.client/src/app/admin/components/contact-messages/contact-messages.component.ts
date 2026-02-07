import { Component, OnInit } from '@angular/core';
import { ContactService } from '../../services/contact.service';
import { ContactMessage } from '../../models/contact.models';

@Component({
  selector: 'app-contact-messages',
  templateUrl: './contact-messages.component.html',
  styleUrls: ['./contact-messages.component.css']
})
export class ContactMessagesComponent implements OnInit {
  allMessages: ContactMessage[] = [];
  filteredMessages: ContactMessage[] = [];
  loading = false;
  error = '';
  selectedMessage: ContactMessage | null = null;

  // Filters
  filterStatus: 'all' | 'answered' | 'unanswered' = 'all';
  filterUserType: 'all' | 0 | 1 = 'all'; // 0=Farmer, 1=Factory
  filterContactType: 'all' | 0 | 1 | 2 | 3 = 'all'; // 0=General, 1=Issue, 2=Suggest, 3=Complaint
  filterDateFrom = '';
  filterDateTo = '';

  constructor(private contactService: ContactService) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(): void {
    this.loading = true;
    this.error = '';
    this.contactService.getAllMessages().subscribe({
      next: (data) => {
        this.allMessages = data;
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.error = 'فشل تحميل الرسائل';
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    let result = [...this.allMessages];

    // Filter by answered status
    if (this.filterStatus === 'answered') {
      result = result.filter(m => m.adminResponse);
    } else if (this.filterStatus === 'unanswered') {
      result = result.filter(m => !m.adminResponse);
    }

    // Filter by user type
    if (this.filterUserType !== 'all') {
      result = result.filter(m => Number(m.userType) === this.filterUserType);
    }

    // Filter by contact type
    if (this.filterContactType !== 'all') {
      result = result.filter(m => Number(m.contactType) === this.filterContactType);
    }

    // Filter by date range
    if (this.filterDateFrom) {
      const from = new Date(this.filterDateFrom);
      result = result.filter(m => new Date(m.submittedAt) >= from);
    }
    if (this.filterDateTo) {
      const to = new Date(this.filterDateTo);
      to.setHours(23, 59, 59);
      result = result.filter(m => new Date(m.submittedAt) <= to);
    }

    this.filteredMessages = result;
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  clearFilters(): void {
    this.filterStatus = 'all';
    this.filterUserType = 'all';
    this.filterContactType = 'all';
    this.filterDateFrom = '';
    this.filterDateTo = '';
    this.applyFilters();
  }

  selectMessage(msg: ContactMessage): void {
    this.selectedMessage = this.selectedMessage?.id === msg.id ? null : msg;
  }

  // Analytics
  get totalCount(): number { return this.allMessages.length; }
  get answeredCount(): number { return this.allMessages.filter(m => m.adminResponse).length; }
  get unansweredCount(): number { return this.allMessages.filter(m => !m.adminResponse).length; }
  get answerRate(): number {
    return this.totalCount > 0 ? Math.round((this.answeredCount / this.totalCount) * 100) : 0;
  }

  getContactTypeLabel(type: string | number): string {
    const map: { [key: string]: string } = {
      '0': 'عام', '1': 'مشكلة', '2': 'اقتراح', '3': 'شكوى',
      'General': 'عام', 'Issue': 'مشكلة', 'Suggest': 'اقتراح', 'Complaint': 'شكوى'
    };
    return map[String(type)] || String(type);
  }

  getUserTypeLabel(type: string | number): string {
    const map: { [key: string]: string } = {
      '0': 'مزارع', '1': 'مصنع', '2': 'إدارة',
      'Farmer': 'مزارع', 'Factory': 'مصنع', 'Admin': 'إدارة'
    };
    return map[String(type)] || String(type);
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('ar-EG', {
      year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }
}
