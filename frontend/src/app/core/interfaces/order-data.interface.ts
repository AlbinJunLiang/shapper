import { ExtraData } from "./extra-data.interface";
import { OrderDetail } from "./order-detail.interface";

export interface OrderResponse {
    id: number;
    orderReference: string;
    shippingCost: number;
    total: number;
    totalDiscount: number;
    totalTax: number;
    subtotal: number;
    customerId: number;
    status: 'PENDING' | 'COMPLETED' | 'CANCELLED' | 'PAID' | string; // Puedes tipar los estados conocidos
    extraData: ExtraData;
    companyName: string | null;
    createdAt: string;
    details: OrderDetail[];
}