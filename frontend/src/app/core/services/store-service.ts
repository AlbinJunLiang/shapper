import { HttpClient, HttpContext } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { finalize, map, Observable, tap, throwError } from 'rxjs';
import { ApiResponse } from '../interfaces/api-response.interface';
import { StoreData } from '../interfaces/store-data.interface';
import { StoreLink } from '../interfaces/store-link.interface';
import { StoreUpdate } from '../interfaces/store-update.interface';
import { IS_PUBLIC } from '../constants/http-tokens';
@Injectable({
    providedIn: 'root',
})
export class StoreService {
    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);
    public storeData = signal<StoreData | null>(null);

    private _isLoading = signal<boolean>(false); // Privado para el servicio
    public isLoading = this._isLoading.asReadonly(); // Público para los componentes

    storeLinks = computed(() => this.storeData()?.storeLinks || []);

    loadStore(storeCode: string) {
        if (storeCode === "" || storeCode === null) return; // Validación básica

        this._isLoading.set(true);

        this.http.get<ApiResponse<StoreData>>(`${this.apiUrl}/Store/code/${storeCode}`, {
            context: new HttpContext().set(IS_PUBLIC, true)
        }).pipe(
            map(res => res.data),
            finalize(() => this._isLoading.set(false))
        ).subscribe({
            next: (data) => this.storeData.set(data),

            error: (err) => {
                this.storeData.set(null);
            }
        });
    }


    updateStore(id: number, storeData: StoreUpdate): Observable<ApiResponse<void>> {
        if (!id) {
            return throwError(() => new Error('Id is required.'));
        }
        this._isLoading.set(true);
        return this.http.put<ApiResponse<void>>(`${this.apiUrl}/Store/${id}`, storeData).pipe(
            tap(() => {
                this.storeData.update(current =>
                    current ? { ...current, ...storeData } : null
                );
            }),
            finalize(() => this._isLoading.set(false))
        );
    }

    getLink(type: string, name: string): StoreLink | undefined {
        // 1. Obtenemos los links de la señal
        const links = this.storeLinks();

        return links.find(link =>
            link.type?.trim().toLowerCase() === type.trim().toLowerCase() &&
            link.name?.trim().toLowerCase() === name.trim().toLowerCase()
        );
    }

    setStoreData(newData: StoreData) {
        this.storeData.set(newData);
    }

    clearStore() {
        this.storeData.set(null);
    }
}