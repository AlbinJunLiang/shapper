import { CategoriesResponse } from "./category.interface";

export interface FilterCategoryResponse {
  globalMinPrice: number;
  globalMaxPrice: number;
  categories: CategoriesResponse[];
}