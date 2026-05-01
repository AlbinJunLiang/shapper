import { computed, inject, Injectable, signal } from "@angular/core";
import { CategoryService } from "./category-service";
import { FilterCategoryResponse } from "../interfaces/filter-category-response.interface";

@Injectable({ providedIn: 'root' })
export class FilterStore {
    private categoryService = inject(CategoryService);

    private _categories = signal<FilterCategoryResponse | undefined>(undefined);
    private _loading = signal<boolean>(false);
    public categories = computed(() => this._categories());
    public selectedMin = signal<number>(0);
    public selectedMax = signal<number>(0);


    loadCategoriesFilter() {
        this._loading.set(true);

        this.categoryService.getCategoriesWithPriceRange().subscribe({
            next: (data) => {
                this._categories.set(data);
                if (this.selectedMax() === 0) {
                    this.selectedMax.set(data.globalMinPrice);
                    this.selectedMax.set(data.globalMaxPrice);
                }
                this._loading.set(false);
            },
            error: (err) => {
                console.error('Error al cargar categorías', err);
                this._loading.set(false);
            }
        });
    }
}