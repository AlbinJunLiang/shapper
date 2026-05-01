import { CategoriesResponse } from "./category.interface";
import { Subcategory } from "./subcategory.interface";

export interface PagedSubcategories {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: Subcategory[]
}