// Sto vraca GET /api/users/trainers/search
export interface TrainerSearchResult {
  id: string;
  username: string;
  firstName: string;
  lastName: string;
  location: string;
  profileImageUrl?: string;
  specialization?: string;
  yearsOfExperience?: number;
  description?: string;
  averageRating: number;
  totalReviews: number;
}

// Filteri za pretragu
export interface TrainerSearchFilters {
  keyword?: string;
  specialization?: string;
  minRating?: number;
  sortBy?: string;   // 'rating' | 'experience' | 'name'
}