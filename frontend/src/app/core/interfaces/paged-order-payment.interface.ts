import { CategoriesResponse } from "./category.interface";
import { OrderPayment } from "./order-payment.interface";

export interface PagedOrderPayment {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: OrderPayment[]
}