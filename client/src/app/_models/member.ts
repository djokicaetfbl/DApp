import { Photo } from './photo';

export interface Member {
  id: number;
  userName: string;
  age: number;
  knowAs?: any;
  created: string;
  lastActive: string;
  gender?: string;
  intodruction?: any;
  lookingFor?: string;
  interests?: string;
  city?: string;
  country?: string;
  photos: Photo[];
  photoUrl?: string;
}
