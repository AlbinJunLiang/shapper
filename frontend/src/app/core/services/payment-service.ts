import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { CheckoutResponse } from '../interfaces/checkout-response.interface';
import { IS_PUBLIC } from '../constants/http-tokens';

@Injectable({
    providedIn: 'root',
})
export class PaymentService {

    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);
    checkoutOrder(orderData: any): Observable<CheckoutResponse> {
        return this.http.post<CheckoutResponse>(`${this.apiUrl}/payment/Checkout`, orderData, {
            context: new HttpContext().set(IS_PUBLIC, true)
        });
    }

    confirmPayment(token: string, provider: string): Observable<any> {
        const url = `${this.apiUrl}/payment/capture-payment`;
        const params = new HttpParams()
            .set('provider', provider)
            .set('token', token);

        return this.http.post<any>(url, {}, {
            params,
            context: new HttpContext().set(IS_PUBLIC, true)
        });
    }
}
