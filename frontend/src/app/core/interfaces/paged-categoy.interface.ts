import { CategoriesResponse } from "./category.interface";

export interface PagedCategories {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: CategoriesResponse[]
}