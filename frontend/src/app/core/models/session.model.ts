export enum TrainingSessionStatus {
  Scheduled = 0,
  Completed = 1,
  Cancelled = 2
}

export interface TrainingSession {
  id: string;
  trainingPlanId: string;
  trainingPlanTitle: string;
  trainerId: string;
  startTime: string;
  endTime: string;
  status: TrainingSessionStatus;
  notes?: string;
}

export interface CreateSessionRequest {
  trainingPlanId: string;
  startTime: string;
  endTime: string;
  notes?: string;
}