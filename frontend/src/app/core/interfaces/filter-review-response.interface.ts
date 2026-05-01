import { PagedReviews } from "./paged-review.interface";
import { ProductReviewStats } from "./product-review-stats.interface";

export interface FilterReviewResponse {
    reviews: PagedReviews;
    productStats: ProductReviewStats;
}