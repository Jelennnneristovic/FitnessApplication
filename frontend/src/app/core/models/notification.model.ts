export enum NotificationType {
  Info = 0,
  Success = 1,
  Warning = 2
}

export interface Notification {
  id: string;
  userId: string;
  title: string;
  content: string;
  isRead: boolean;
  type: NotificationType;
  createdAt: string;
  readAt?: string;
}