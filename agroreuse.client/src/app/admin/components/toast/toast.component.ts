import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { ToastService, Toast } from '../../services/toast.service';

@Component({
  selector: 'app-toast',
  template: `
    <div class="toast-container">
      <div
        *ngFor="let toast of toasts"
        class="toast toast-{{ toast.type }}"
        (click)="toastService.dismiss(toast.id)">
        <span class="toast-icon">
          <svg *ngIf="toast.type === 'success'" viewBox="0 0 24 24" fill="none">
            <path d="M20 6L9 17L4 12" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <svg *ngIf="toast.type === 'error'" viewBox="0 0 24 24" fill="none">
            <path d="M18 6L6 18M6 6L18 18" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
          </svg>
          <svg *ngIf="toast.type === 'info'" viewBox="0 0 24 24" fill="none">
            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="2"/>
            <path d="M12 8V12M12 16H12.01" stroke="currentColor" stroke-width="2" stroke-linecap="round"/>
          </svg>
        </span>
        <span class="toast-message">{{ toast.message }}</span>
        <button class="toast-close" (click)="toastService.dismiss(toast.id)">×</button>
      </div>
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed;
      bottom: 24px;
      left: 24px;
      z-index: 9999;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 360px;
    }
    .toast {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 14px 16px;
      border-radius: 10px;
      font-size: 14px;
      font-weight: 500;
      box-shadow: 0 4px 20px rgba(0,0,0,0.15);
      cursor: pointer;
      animation: slideIn 0.3s ease-out;
      direction: rtl;
    }
    @keyframes slideIn {
      from { opacity: 0; transform: translateY(20px); }
      to   { opacity: 1; transform: translateY(0); }
    }
    .toast-success { background: #1E4D2B; color: #fff; }
    .toast-error   { background: #c62828; color: #fff; }
    .toast-info    { background: #1565c0; color: #fff; }
    .toast-icon svg { width: 18px; height: 18px; flex-shrink: 0; }
    .toast-message { flex: 1; }
    .toast-close {
      background: none; border: none; color: rgba(255,255,255,0.8);
      font-size: 18px; cursor: pointer; padding: 0; line-height: 1;
      flex-shrink: 0;
    }
    .toast-close:hover { color: #fff; }
    @media (max-width: 480px) {
      .toast-container { left: 12px; right: 12px; bottom: 16px; max-width: unset; }
    }
  `]
})
export class ToastComponent implements OnInit, OnDestroy {
  toasts: Toast[] = [];
  private sub!: Subscription;

  constructor(public toastService: ToastService) {}

  ngOnInit(): void {
    this.sub = this.toastService.toasts$.subscribe(t => this.toasts = t);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}
