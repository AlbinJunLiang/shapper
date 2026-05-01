import { UserData } from "./user-data.interface";

export interface PagedUsers {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: UserData[];
}