import { FeaturedProductResponse } from "./featured-product-response.interface";

export interface PagedFeaturedProduct {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: FeaturedProductResponse[];
}