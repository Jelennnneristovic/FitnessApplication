export enum TrainingType {
  Individual = 0,
  Group = 1
}

export enum TrainingPlanStatus {
  Active = 0,
  Archived = 1
}

export interface TrainingPlan {
  id: string;
  trainerId: string;
  categoryId: string;
  categoryName: string;
  title: string;
  description: string;
  type: TrainingType;
  price: number;
  maxParticipants: number;
  durationMinutes: number;
  location?: string;
  imageUrl?: string;
  status: TrainingPlanStatus;
  createdAt: string;
  updatedAt?: string;
}