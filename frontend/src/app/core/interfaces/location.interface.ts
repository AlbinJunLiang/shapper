export interface LocationResponse {
  id: number;
  name: string;
  contact: string;
  address: string;
  type: string;
  cost: number;
  status: 'ACTIVE' | 'INACTIVE';
}

export interface CreateLocation {
  name: string;
  address: string;
  type: string;
  cost: number;
  status: 'ACTIVE' | 'INACTIVE';
}