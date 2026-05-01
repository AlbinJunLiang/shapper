import { RatingCount } from "./rating-count.interface";

export interface ProductReviewStats {
  averageRating: number;
  totalReviews: number;
  ratingStats: RatingCount[]; 
}