export interface TrainerProfile {
  id: string;
  userId: string;
  username: string;
  firstName: string;
  lastName: string;
  profileImageUrl?: string;
  specialization?: string;
  yearsOfExperience?: number;
  description?: string;
  updatedAt?: string;
}

export interface UpdateTrainerProfileRequest {
  specialization?: string;
  yearsOfExperience?: number;
  description?: string;
}