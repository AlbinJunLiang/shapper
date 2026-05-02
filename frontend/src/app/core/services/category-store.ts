import { inject, Injectable, signal, computed } from '@angular/core';
import { CategoriesResponse } from '../interfaces/category.interface';
import { CategoryService } from './category-service';
import { PagedCategories } from '../interfaces/paged-categoy.interface';
import { finalize, tap } from 'rxjs';
import { CategoryRequest } from '../interfaces/category-request.interface';

@Injectable({ providedIn: 'root' })
export class CategoryStore {
    private categoryService = inject(CategoryService);

    private _categories = signal<CategoriesResponse[]>([]);
    private _totalCategories = signal<number>(0);

    public categories = computed(() => this._categories());
    private _loading = signal<boolean>(false);

    public isLoading = computed(() => this._loading());
    public totalCategories = computed(() => this._totalCategories());



    loadCategories(page: number = 1, pageSize: number = 8) {
        if (this._loading()) return;
        this._loading.set(true);

        this.categoryService.getCategories(page, pageSize).subscribe({
            next: (response: PagedCategories) => {
                this.updateCategoriesState(response.data);
                this._loading.set(false);
                this._totalCategories.set(response.totalCount)
            },
            error: () => this._loading.set(false)
        });
    }

    getCategories(page: number = 1, pageSize: number = 8) {
        if (this._loading()) return;
        this._loading.set(true);

        this.categoryService.getCategories(page, pageSize).subscribe({
            next: (response: PagedCategories) => {
                this._categories.set(response.data);
                this._loading.set(false);
                this._totalCategories.set(response.totalCount)
            },
            error: () => this._loading.set(false)
        });
    }


    // En category-store.ts

    createCategory(data: CategoryRequest, imageFile?: File) {
        this._loading.set(true);
        return this.categoryService.createCategory(data, imageFile).pipe(
            tap((response: any) => { // Aquí llega el JSON que me pasaste

                const newCategory = response.data;
                this._categories.update(current => [...current, newCategory]);
                this._totalCategories.update(total => total + 1);
            }),
            finalize(() => this._loading.set(false))
        );
    }


    // En category-store.ts
    deleteCategory(id: number) {
        this._loading.set(true);
        return this.categoryService.deleteCategory(id).pipe(
            tap(() => {
                this._categories.update(current => current.filter(cat => cat.id !== id));
                this._totalCategories.update(total => total - 1);
            }),
            finalize(() => this._loading.set(false))
        );
    }


    // En category-store.ts

    updateCategory(id: number, data: CategoryRequest, imageFile?: File) {
        this._loading.set(true);

        return this.categoryService.updateCategory(id, data, imageFile).pipe(
            tap((response: any) => {
                const updatedCategory = response.data;
                this._categories.update(current =>
                    current.map(cat => cat.id === id ? updatedCategory : cat)
                );
            }),
            finalize(() => this._loading.set(false))
        );
    }

    /**
     * FUNCIÓN PRIVADA DE MEZCLA
     * Se encarga solo de la lógica de actualización del array
     */
    private updateCategoriesState(newCategories: CategoriesResponse[]) {
        this._categories.update(prev => {
            // 1. Filtramos las nuevas: solo dejamos las que NO están en 'prev'
            const uniqueNew = newCategories.filter(
                newCat => !prev.some(oldCat => oldCat.id === newCat.id)
            );
            // 2. Retornamos la unión de lo viejo con lo verdaderamente nuevo
            return [...prev, ...uniqueNew];
        });
    }

    setCategories(newData: PagedCategories) {
        this._categories.set(newData.data);
        this._totalCategories.set(newData.totalCount);
    }
}