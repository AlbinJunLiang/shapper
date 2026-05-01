import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { catchError, Observable, throwError } from 'rxjs';
import { IS_PUBLIC } from '../constants/http-tokens';
import { OrderResponse } from '../interfaces/order-data.interface';

@Injectable({
    providedIn: 'root',
})
export class OrderService {

    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);

    /**
       * Crea una nueva orden en el sistema
       * @param orderData Los datos del formulario de checkout
       */
    createOrder(orderData: any): Observable<OrderResponse> {
        return this.http.post<OrderResponse>(`${this.apiUrl}/Orders`, orderData, {
            context: new HttpContext().set(IS_PUBLIC, true)
        });
    }

    getOrderByUserId(page: number = 1, pageSize: number = 10, userId: number) {
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<any>(`${this.apiUrl}/Orders/user/${userId}`)
            .pipe(
                catchError((err) => {
                    return throwError(() => err);
                })
            );
    }


    getOrders(page: number = 1, pageSize: number = 10) {
        // Usar HttpParams es mejor que concatenar strings para evitar errores de formato
        const params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());
        return this.http.get<any>(`${this.apiUrl}/Orders`, {
            params, context: new HttpContext().set(IS_PUBLIC, true)
        })
            .pipe(
                catchError((err) => {
                    return throwError(() => err);
                })
            );
    }

    getOrderByReference(reference: string): Observable<OrderResponse> {
        return this.http.get<OrderResponse>(`${this.apiUrl}/Orders/reference/${reference}`);
    }


    /**
   * Actualiza el estado de una orden
   * @param id El ID numérico de la orden
   * @param status El nuevo estado (PENDING, SHIPPED, etc.)
   */
    updateOrderStatus(id: number, status: string) {
        // Usamos HttpParams para construir la query string de forma segura
        const params = new HttpParams().set('status', status);
        return this.http.patch<{ success: boolean; message: string }>(
            `${this.apiUrl}/Orders/status/${id}`,
            {}, // El cuerpo (body) va vacío porque usas [FromQuery]
            { params }
        );
    }
}
