export interface Attendance {
  id: string;
  trainingSessionId: string;
  sessionStartTime: string;
  trainingPlanTitle: string;
  clientId: string;
  attended: boolean;
  markedAt: string;
  markedByUserId: string;
  notes?: string;
}

export interface MarkAttendanceRequest {
  attended: boolean;
  notes?: string;
}