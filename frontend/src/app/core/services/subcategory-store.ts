import { computed, inject, Injectable, signal } from "@angular/core";
import { SubcategoryService } from "./subcategory-service";
import { PagedCategories } from "../interfaces/paged-categoy.interface";
import { Subcategory } from "../interfaces/subcategory.interface";
import { SubcategoryRequest } from "../interfaces/subcategory-request.interface";
import { finalize, tap } from "rxjs";

@Injectable({ providedIn: 'root' })
export class SubcategoryStore {
    private subcategoryService = inject(SubcategoryService);

    private _subcategories = signal<Subcategory[]>([]);
    private _loading = signal<boolean>(false);
    private _totalSubcategories = signal<number>(0);

    public subcategories = computed(() => this._subcategories());
    public isLoading = computed(() => this._loading());
    public totalSubcategories = computed(() => this._totalSubcategories());


    getSubcategories(page: number = 1, pageSize: number = 100) {
        if (this._loading()) return;
        this._loading.set(true);

        this.subcategoryService.getSubcategories(page, pageSize).subscribe({
            next: (response: PagedCategories) => {
                this._subcategories.set(response.data);
                this._loading.set(false);
                this._totalSubcategories.set(response.totalCount)
            },
            error: () => this._loading.set(false)
        });
    }


    createSubcategory(data: SubcategoryRequest, imageFile?: File) {
        this._loading.set(true);
        return this.subcategoryService.createSubcategory(data, imageFile).pipe(
            tap((response: any) => {

                const newCategory = response.data;
                this._subcategories.update(current => [...current, newCategory]);
                this._totalSubcategories.update(total => total + 1);
            }),
            finalize(() => this._loading.set(false))
        );
    }


    deleteSubcategory(id: number) {
        this._loading.set(true);
        return this.subcategoryService.deleteSubcategory(id).pipe(
            tap(() => {
                this._subcategories.update(current => current.filter(cat => cat.id !== id));
                this._totalSubcategories.update(total => total - 1);
            }),
            finalize(() => this._loading.set(false))
        );
    }

    updateCategory(id: number, data: SubcategoryRequest, imageFile?: File) {
        this._loading.set(true);

        return this.subcategoryService.updateSubcategory(id, data, imageFile).pipe(
            tap((response: any) => {
                const updatedCategory = response.data;
                this._subcategories.update(current =>
                    current.map(cat => cat.id === id ? updatedCategory : cat)
                );
            }),
            finalize(() => this._loading.set(false))
        );
    }



}