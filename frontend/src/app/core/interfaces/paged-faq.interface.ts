import { IFaq } from "./faq.interface";

export interface PagedFaq {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: IFaq[];
}