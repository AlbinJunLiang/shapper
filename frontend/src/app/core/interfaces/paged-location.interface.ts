import { LocationResponse } from "./location.interface";

export interface PagedLocations {
    totalCount: number;
    totalPages: number;
    page: number;
    pageSize: number;
    data: LocationResponse[];
}