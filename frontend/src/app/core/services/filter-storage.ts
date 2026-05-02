import { computed, inject, Injectable, signal } from "@angular/core";
import { CategoryService } from "./category-service";
import { FilterCategoryResponse } from "../interfaces/filter-category-response.interface";


@Injectable({ providedIn: 'root' })
export class FilterStore {
    private categoryService = inject(CategoryService);

    // Estado Privado
    private _categories = signal<FilterCategoryResponse | undefined>(undefined);
    private _loading = signal<boolean>(false);

    // Estado Público (Solo lectura)
    public categories = computed(() => this._categories());
    public isLoading = computed(() => this._loading());

    public selectedMin = signal<number>(0);
    public selectedMax = signal<number>(0);

    loadCategoriesFilter() {
        if (this._loading()) return; // Evitar múltiples llamadas simultáneas

        this._loading.set(true);
        this.categoryService.getCategoriesWithPriceRange().subscribe({
            next: (data) => {
                this._categories.set(data);
                this.initializePriceRange(data);
                this._loading.set(false);
            },
            error: (err) => {
                console.error('Error al cargar categorías', err);
                this._loading.set(false);
            }
        });
    }

    private initializePriceRange(data: FilterCategoryResponse) {
        // Solo inicializamos si el rango actual es 0 (primera carga)
        if (this.selectedMax() === 0) {
            this.selectedMin.set(data.globalMinPrice);
            this.selectedMax.set(data.globalMaxPrice);
        }
    }

    // Útil para resetear filtros si fuera necesario
    resetFilters() {
        const data = this._categories();
        if (data) {
            this.selectedMin.set(data.globalMinPrice);
            this.selectedMax.set(data.globalMaxPrice);
        }
    }

    // En FilterStore
    setCategories(newData: FilterCategoryResponse) {
        this._categories.set(newData);
        this.selectedMin.set(newData.globalMinPrice);
        this.selectedMax.set(newData.globalMaxPrice);
    }

}