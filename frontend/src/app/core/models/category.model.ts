export interface Category {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

// Sta saljemo pri kreiranju
export interface CreateCategoryRequest {
  name: string;
  description?: string;
}

// Sta saljemo pri izmeni (sva polja opciona)
export interface UpdateCategoryRequest {
  name?: string;
  description?: string;
  isActive?: boolean;
}