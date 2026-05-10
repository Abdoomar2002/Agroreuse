import { Component, OnInit } from '@angular/core';
import { ContactService } from '../../services/contact.service';
import { ContactMessage } from '../../models/contact.models';
import { ToastService } from '../../services/toast.service';
import { SortUtil, SortDirection } from '../../utilities/sort.util';

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

  // Respond modal
  showRespondModal = false;
  respondingMessage: ContactMessage | null = null;
  responseText = '';
  respondLoading = false;
  respondError = '';

  // Action loading states
  markingReadId: string | null = null;
  deletingId: string | null = null;

  // Filters
  filterStatus: 'all' | 'answered' | 'unanswered' = 'all';
  filterUserType: 'all' | 0 | 1 = 'all';
  filterContactType: 'all' | 0 | 1 | 2 | 3 = 'all';
  filterDateFrom = '';
  filterDateTo = '';

  // Sorting
  sortColumn: keyof ContactMessage | null = null;
  sortDirection: SortDirection = null;

  constructor(private contactService: ContactService, private toast: ToastService) {}

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

    if (this.filterStatus === 'answered') {
      result = result.filter(m => m.adminResponse);
    } else if (this.filterStatus === 'unanswered') {
      result = result.filter(m => !m.adminResponse);
    }

    if (this.filterUserType !== 'all') {
      result = result.filter(m => Number(m.userType) === this.filterUserType);
    }

    if (this.filterContactType !== 'all') {
      result = result.filter(m => Number(m.contactType) === this.filterContactType);
    }

    if (this.filterDateFrom) {
      const from = new Date(this.filterDateFrom);
      result = result.filter(m => new Date(m.submittedAt) >= from);
    }
    if (this.filterDateTo) {
      const to = new Date(this.filterDateTo);
      to.setHours(23, 59, 59);
      result = result.filter(m => new Date(m.submittedAt) <= to);
    }

    // Apply sorting
    if (this.sortColumn) {
      result = SortUtil.sort(result, this.sortColumn, this.sortDirection, this.sortColumn, this.sortDirection);
    }

    this.filteredMessages = result;
  }

  onFilterChange(): void { this.applyFilters(); }

  clearFilters(): void {
    this.filterStatus = 'all';
    this.filterUserType = 'all';
    this.filterContactType = 'all';
    this.filterDateFrom = '';
    this.filterDateTo = '';
    this.applyFilters();
  }

  onSort(column: keyof ContactMessage): void {
    const newSortState = SortUtil.toggleSort(column as string, this.sortColumn as string | null, this.sortDirection);
    this.sortColumn = newSortState.column as keyof ContactMessage | null;
    this.sortDirection = newSortState.direction;
    this.applyFilters();
  }

  isSortedBy(column: keyof ContactMessage): boolean {
    return this.sortColumn === column;
  }

  getSortIcon(column: keyof ContactMessage): string {
    if (this.sortColumn !== column) return '⇅';
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  selectMessage(msg: ContactMessage): void {
    this.selectedMessage = this.selectedMessage?.id === msg.id ? null : msg;
  }

  // ── Mark as read ──────────────────────────────────────────────────────────
  markAsRead(msg: ContactMessage, event: Event): void {
    event.stopPropagation();
    if (msg.isRead) return;
    this.markingReadId = msg.id;
    this.contactService.markAsRead(msg.id).subscribe({
      next: () => {
        msg.isRead = true;
        this.markingReadId = null;
        if (this.selectedMessage?.id === msg.id) {
          this.selectedMessage = { ...msg, isRead: true };
        }
        this.toast.success('تم تعيين الرسالة كمقروءة');
      },
      error: () => { this.markingReadId = null; this.toast.error('فشل تحديث حالة القراءة'); }
    });
  }

  // ── Respond modal ─────────────────────────────────────────────────────────
  openRespondModal(msg: ContactMessage, event: Event): void {
    event.stopPropagation();
    this.respondingMessage = msg;
    this.responseText = msg.adminResponse ?? '';
    this.respondError = '';
    this.showRespondModal = true;
  }

  closeRespondModal(): void {
    this.showRespondModal = false;
    this.respondingMessage = null;
    this.responseText = '';
    this.respondError = '';
  }

  submitResponse(): void {
    if (!this.responseText.trim() || !this.respondingMessage) return;
    this.respondLoading = true;
    this.respondError = '';
    this.contactService.respondToMessage(this.respondingMessage.id, this.responseText.trim()).subscribe({
      next: (res) => {
        const updated: ContactMessage = res?.data ?? {
          ...this.respondingMessage!,
          adminResponse: this.responseText.trim(),
          respondedAt: new Date().toISOString(),
          isRead: true
        };
        const idx = this.allMessages.findIndex(m => m.id === updated.id);
        if (idx !== -1) this.allMessages[idx] = updated;
        if (this.selectedMessage?.id === updated.id) this.selectedMessage = updated;
        this.applyFilters();
        this.respondLoading = false;
        this.closeRespondModal();
        this.toast.success('تم إرسال الرد بنجاح وسيتلقى المستخدم إشعارًا');
      },
      error: () => {
        this.respondError = 'فشل إرسال الرد. حاول مرة أخرى.';
        this.respondLoading = false;
        this.toast.error('فشل إرسال الرد. حاول مرة أخرى.');
      }
    });
  }

  // ── Delete ────────────────────────────────────────────────────────────────
  deleteMessage(msg: ContactMessage, event: Event): void {
    event.stopPropagation();
    if (!confirm('هل أنت متأكد من حذف هذه الرسالة؟')) return;
    this.deletingId = msg.id;
    this.contactService.deleteMessage(msg.id).subscribe({
      next: () => {
        this.allMessages = this.allMessages.filter(m => m.id !== msg.id);
        if (this.selectedMessage?.id === msg.id) this.selectedMessage = null;
        this.applyFilters();
        this.deletingId = null;
        this.toast.success('تم حذف الرسالة بنجاح');
      },
      error: () => { this.deletingId = null; this.toast.error('فشل حذف الرسالة'); }
    });
  }

  // ── Analytics ─────────────────────────────────────────────────────────────
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

  formatDate(date?: string): string {
    if (!date) return '';
    return new Date(date).toLocaleDateString('ar-EG', {
      year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit'
    });
  }
}
