export interface ContactMessage {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  userPhone?: string;
  userType: string;
  contactType: string;
  message: string;
  submittedAt: string;
  isRead: boolean;
  adminResponse?: string;
  respondedAt?: string;
}
