import { Product } from "./product.interface";

export interface PagedProduct {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data : Product[]
}