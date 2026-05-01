import { ReviewResponse } from "./review-response.interface";

export interface PagedReviews {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: ReviewResponse[];
}