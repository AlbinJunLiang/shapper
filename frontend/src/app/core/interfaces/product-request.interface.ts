import { ProductStatus } from "../enums/product-status.enum";

export interface ProductRequest {
    subcategoryId: number,
    name: string;
    description: string;
    price: number;
    taxAmount: number;
    discount: number;
    details: string;
    quantity: number;
    status:  ProductStatus;
}