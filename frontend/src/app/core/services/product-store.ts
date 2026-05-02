import { inject, Injectable, signal, computed } from '@angular/core';
import { Product } from '../interfaces/product.interface';
import { ProductService } from './product-service';
import { PagedProduct } from '../interfaces/paged-product.interface';
import { ProductRequest } from '../interfaces/product-request.interface';
import { finalize, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductStore {
    private productService = inject(ProductService);
    private _apiResponse = signal<PagedProduct>({
        totalCount: 0,
        totalPages: 0,
        page: 1,
        pageSize: 10,
        data: []
    });

    public apiResponse = this._apiResponse.asReadonly();
    public products = computed(() => this._apiResponse().data);
    public pagination = computed(() => ({
        current: this._apiResponse().page,
        total: this._apiResponse().totalPages
    }));

    private _loading = signal<boolean>(false);
    public isLoading = this._loading.asReadonly();

    /**
     * Carga normal (Paginada)
     */
    loadProducts(page: number = 1, pageSize: number = 10, onlyFeatured: boolean = false) {
        if (this._loading()) return;
        this._loading.set(true);

        this.productService.getProductsStore(page, pageSize, onlyFeatured).subscribe({
            next: (response) => {
                this._apiResponse.set(response);
            },
            error: (err) => console.error('Error Shapper API:', err),
            complete: () => this._loading.set(false)
        });
    }

    loadProductsAdmin(page: number = 1, pageSize: number = 10, onlyFeatured: boolean = false) {
        if (this._loading()) return;
        this._loading.set(true);
        this.productService.getProductsAdmin(page, pageSize, onlyFeatured).subscribe({
            next: (response) => {
                this._apiResponse.set(response);
            },
            error: (err) => console.error('Error Shapper API:', err),
            complete: () => this._loading.set(false)
        });
    }


    loadFilterProducts(filters: any) {
        if (this._loading()) return;
        this._loading.set(true);

        this.productService.filterProducts(filters).subscribe({
            next: (response) => {
                this._apiResponse.set(response);

            },
            error: (err) => {
                console.error('Error Shapper API:', err);
                this._loading.set(false);
            },
            complete: () => {
                this._loading.set(false);
            }
        });
    }

    /**
     * Búsqueda (Actualiza la lista sin romper la paginación global)
     */
    loadSearchProducts(term: string, count: number = 5) {
        if (this._loading() || !term.trim()) return;
        this._loading.set(true);

        this.productService.searchProducts(term, count).subscribe({
            next: (response) => {
                const results = Array.isArray(response) ? response : [response];
                this.updateLocalProducts(results);
            },
            complete: () => this._loading.set(false)
        });
    }



    setSearchProducts(term: string, count: number = 5) {
        if (this._loading() || !term.trim()) return;
        this._loading.set(true);

        this.productService.searchProducts(term, count).subscribe({
            next: (response) => {
                this._apiResponse.update(state => ({
                    ...state,
                    data: [...response] // Reemplazo directo
                }));
            },
            complete: () => this._loading.set(false)
        });
    }

    /**
     * Actualiza o inserta productos en la lista actual 
     * SIN resetear los contadores de página de la base de datos.
     */
    updateLocalProducts(newProducts: Product[]) {
        this._apiResponse.update(state => {
            const currentItems = [...state.data];

            newProducts.forEach(newItem => {
                const index = currentItems.findIndex(p => p.id === newItem.id);
                if (index !== -1) {
                    currentItems[index] = { ...currentItems[index], ...newItem };
                } else {
                    currentItems.unshift(newItem); // Insertar al inicio para que el usuario lo vea
                }
            });

            return { ...state, data: currentItems };
        });
    }


    createProduct(data: ProductRequest) {
        this._loading.set(true);
        return this.productService.createProduct(data).pipe(
            tap((response: Product) => {
            }),
            finalize(() => this._loading.set(false))
        );
    }

    deleteProduct(id: number) {
        this._loading.set(true);

        return this.productService.deleteProduct(id).pipe(
            tap(() => {
                this._apiResponse.update(state => ({
                    ...state,
                    totalCount: state.totalCount - 1,
                    data: state.data.filter(p => p.id !== id)
                }));
            }),
            finalize(() => this._loading.set(false))
        );
    }


 updateProduct(data: ProductRequest, productId: number) {
  this._loading.set(true);

  return this.productService.updateProduct(data, productId).pipe(
    tap(() => {
      this._apiResponse.update(state => ({
        ...state,
        data: state.data.map(p =>
          p.id === productId
            ? {
                ...p,        // mantienes Product completo
                ...data      // aplicas ProductRequest encima
              }
            : p
        )
      }));
    }),
    finalize(() => this._loading.set(false))
  );
}
}