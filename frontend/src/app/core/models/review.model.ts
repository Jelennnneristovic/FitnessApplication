// Sto vraca GET /api/trainers/{id}/reviews
export interface Review {
  id: string;
  trainerId: string;
  clientId: string;
  clientUsername: string;
  clientFirstName: string;
  clientLastName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

// Sta saljemo pri kreiranju ocene
export interface CreateReviewRequest {
  rating: number;
  comment?: string;
}