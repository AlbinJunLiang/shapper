
export interface ReviewResponse {
    id: number;
    productId: number;
    userId: number;
    name: string;
    lastName: string;
    rating: number;
    comment: string;
    createdAt: Date;
    status: string;
}
