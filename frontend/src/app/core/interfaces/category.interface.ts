import { Subcategory } from "./subcategory.interface";

export interface CategoriesResponse {
  id: number;
  name: string;
  description: string;
  imageUrl: string;
  imageProvider? : string;
  subcategories?: Subcategory[]; 
  completed?: boolean;
}