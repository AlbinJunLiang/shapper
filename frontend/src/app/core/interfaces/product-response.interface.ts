import { Product } from "./product.interface";

export interface ProductResponse {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data : Product[]
}