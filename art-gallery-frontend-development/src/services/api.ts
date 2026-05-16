import axios from 'axios';
import type { Artifact, Artist, ArtType, AuthResponse, ApiResponse } from '../types';

const API_BASE = 'http://localhost:5027/api';

const api = axios.create({
  baseURL: API_BASE,
  headers: { 'Content-Type': 'application/json' },
});

// Attach JWT token to every request if available
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Helper to unwrap ApiResponse<T>
const unwrap = <T>(res: { data: ApiResponse<T> }): T => res.data.data;

// ── Auth ─────────────────────────────────────────────────────────────────────
export const login = (email: string, password: string) =>
  api.post<ApiResponse<AuthResponse>>('/auth/login', { email, password }).then(unwrap);

export const register = (email: string, password: string, firstName: string, lastName: string) =>
  api.post<ApiResponse<AuthResponse>>('/auth/register', { email, password, firstName, lastName }).then(unwrap);

// ── Artifacts ─────────────────────────────────────────────────────────────────
export const getArtifacts = () =>
  api.get<ApiResponse<Artifact[]>>('/artifacts').then(unwrap);

export const getArtifact = (id: number) =>
  api.get<ApiResponse<Artifact>>(`/artifacts/${id}`).then(unwrap);

export const createArtifact = (data: Partial<Artifact> & { artistId: number; artTypeId: number }) =>
  api.post<ApiResponse<Artifact>>('/artifacts', data).then(unwrap);

export const updateArtifact = (id: number, data: Partial<Artifact> & { artistId: number; artTypeId: number }) =>
  api.put<ApiResponse<Artifact>>(`/artifacts/${id}`, { id, ...data }).then(unwrap);

export const deleteArtifact = (id: number) =>
  api.delete<ApiResponse<boolean>>(`/artifacts/${id}`).then(unwrap);

// ── Artists ───────────────────────────────────────────────────────────────────
export const getArtists = () =>
  api.get<ApiResponse<Artist[]>>('/artists').then(unwrap);

export const getArtist = (id: number) =>
  api.get<ApiResponse<Artist>>(`/artists/${id}`).then(unwrap);

export const createArtist = (data: Omit<Artist, 'id' | 'fullName' | 'createdAt' | 'artifactCount'>) =>
  api.post<ApiResponse<Artist>>('/artists', data).then(unwrap);

export const updateArtist = (id: number, data: Partial<Artist>) =>
  api.put<ApiResponse<Artist>>(`/artists/${id}`, { id, ...data }).then(unwrap);

export const deleteArtist = (id: number) =>
  api.delete<ApiResponse<boolean>>(`/artists/${id}`).then(unwrap);

// ── Art Types (Categories) ────────────────────────────────────────────────────
export const getArtTypes = () =>
  api.get<ApiResponse<ArtType[]>>('/arttypes').then(unwrap);

export const getArtType = (id: number) =>
  api.get<ApiResponse<ArtType>>(`/arttypes/${id}`).then(unwrap);

export const createArtType = (data: Omit<ArtType, 'id' | 'artifactCount'>) =>
  api.post<ApiResponse<ArtType>>('/arttypes', data).then(unwrap);

export const updateArtType = (id: number, data: Partial<ArtType>) =>
  api.put<ApiResponse<ArtType>>(`/arttypes/${id}`, { id, ...data }).then(unwrap);

export const deleteArtType = (id: number) =>
  api.delete<ApiResponse<boolean>>(`/arttypes/${id}`).then(unwrap);
