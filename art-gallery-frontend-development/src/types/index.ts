// Types aligned with the backend DTOs

export interface Artifact {
  id: number;
  title: string;
  description?: string;
  story?: string;
  dimensions?: string;
  medium?: string;
  yearCreated?: number;
  price?: number;
  isAvailable: boolean;
  imageUrl?: string;
  createdAt: string;
  artist?: ArtistSummary;
  artType?: ArtTypeSummary;
}

export interface ArtistSummary {
  id: number;
  fullName: string;
  region?: string;
}

export interface ArtTypeSummary {
  id: number;
  name: string;
  technique: string;
}

export interface Artist {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  clanGroup?: string;
  region?: string;
  languageGroup?: string;
  birthDate?: string;
  deathDate?: string;
  biography?: string;
  isDeceased: boolean;
  createdAt: string;
  artifactCount: number;
}

export interface ArtType {
  id: number;
  name: string;
  description: string;
  region: string;
  technique: string;
  artifactCount: number;
}

export interface AuthResponse {
  token: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
  errors?: string[];
}

export type Section = 'dashboard' | 'artifacts' | 'artists' | 'artTypes' | 'auth';
