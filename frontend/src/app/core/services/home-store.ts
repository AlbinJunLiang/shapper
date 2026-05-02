import { computed, inject, Injectable, signal } from "@angular/core";
import { Product } from "../interfaces/product.interface";
import { environment } from "../../../environments/environment.development";
import { HomeService } from "./home-service";
import { ResumeInterface } from "../interfaces/home.interface";
import { StoreService } from "./store-service";
import { ProductStore } from "./product-store";
import { FilterStore } from "./filter-storage";
import { CategoryStore } from "./category-store";

@Injectable({
    providedIn: 'root'
})
export class HomeStore {

    private homeService = inject(HomeService);
    private _featuredProducts = signal<Product[]>([]);
    public featuredProducts = this._featuredProducts.asReadonly();

    private storeService = inject(StoreService);
    private productStore = inject(ProductStore);
    private filterStore = inject(FilterStore);
    private categoryStore = inject(CategoryStore);


    private _loading = signal<boolean>(false);
    public isLoading = computed(() => this._loading());

    loadHomeData(
        storeId: string = environment.storeReference,
        page: number = 1,
        pageSize: number = 20,
        categoriesPage: number = 1,
        categoriesPageSize: number = 8,
        featured: boolean = true) {
        if (this._loading()) return;
        this._loading.set(true);

        this.homeService.getHomeData(storeId, page, pageSize, categoriesPage, categoriesPageSize, featured).subscribe({
            next: (response: ResumeInterface) => {
                this._featuredProducts.set(response.data.products.data);
                this.storeService.setStoreData(response.data.storeInfo);
                this.productStore.updateLocalProducts(response.data.products.data);
                this.filterStore.setCategories(response.data.categoriesWithPriceRange);
                this.categoryStore.setCategories(response.data.categories);
                this._loading.set(false);
            },
            error: () => this._loading.set(false)
        });
    }



}