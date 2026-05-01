import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { StoreLinkUpdate } from '../interfaces/store-link-update.interface';
import { StoreLink } from '../interfaces/store-link.interface';
import { ApiResponse } from '../interfaces/api-response.interface';

@Injectable({
    providedIn: 'root'
})
export class StoreLinkService {
    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);

    /**
     * Upsert: Crea o actualiza un StoreLink
     * @param id El ID del link (puede ser -5, 0 o null para nuevos)
     * @param dto Los datos del link
     */
    upsertStoreLink(id: number | null, dto: StoreLinkUpdate): Observable<StoreLink> {
        const url = id !== null ? `${this.apiUrl}/Storelinks/upsert/${id}` : `${this.apiUrl}/StoreLinks/upsert`;
        const headers = new HttpHeaders({
            'accept': '*/*',
            'Content-Type': 'application/json'
        });
        return this.http.put<ApiResponse<StoreLink>>(url, dto, { headers }).pipe(
            map(response => {
                if (response.success) {
                    return response.data;
                } else {
                    // Esto caerá en el bloque 'error' del subscribe
                    throw new Error(response.message);
                }
            })
        );
    }

    toggleStatus(id: number): Observable<StoreLink> {
        const url = `${this.apiUrl}/Storelinks/toggle-status/${id}`;
        return this.http.patch<ApiResponse<StoreLink>>(url, {}).pipe(
            map(response => {
                if (response.success) {
                    return response.data;
                } else {
                    throw new Error(response.message);
                }
            })
        );
    }

}