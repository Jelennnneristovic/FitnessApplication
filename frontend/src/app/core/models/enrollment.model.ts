export enum EnrollmentStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
  Cancelled = 3
}

export interface CreateEnrollmentRequest {
  trainingPlanId: string;
  clientNote?: string;
}

export interface Enrollment {
  id: string;
  trainingPlanId: string;
  trainingPlanTitle: string;
  trainerId: string;
  clientId: string;
  status: EnrollmentStatus;
  requestedAt: string;
  respondedAt?: string;
  rejectionReason?: string;
  clientNote?: string;
}