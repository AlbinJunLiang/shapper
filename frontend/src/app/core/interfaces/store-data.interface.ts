import { StoreLink } from "./store-link.interface";

export interface StoreData {
  id: number;
  name: string;
  description: string;
  storeCode: string;
  email: string;
  phoneNumber: string;
  createdAt: string;
  mainLocation: string;
  location: any | null;
  storeLinks: StoreLink[];
}