import { ProductImage } from "./product-image.interface";

export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    newPrice: number;
    taxAmount: number;
    discount: number;
    details: string;
    quantity: number;
    status: 'ACTIVE' | 'INACTIVE' | 'DISCONTINUED';
    subcategoryName?: string;
    subcategoryId?: number;

    images: ProductImage[];
}