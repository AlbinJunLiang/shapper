import { FilterCategoryResponse } from "./filter-category-response.interface";
import { PagedCategories } from "./paged-categoy.interface";
import { PagedProduct } from "./paged-product.interface";
import { StoreData } from "./store-data.interface";

export interface HomeData {
    storeInfo: StoreData;
    products: PagedProduct;
    categories: PagedCategories;
    categoriesWithPriceRange: FilterCategoryResponse;
    timestamp: Date;

}

export interface ResumeInterface {
    success: boolean;
    data: HomeData;
}