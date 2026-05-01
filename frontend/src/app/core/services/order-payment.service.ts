import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment.development";
import { HttpClient } from "@angular/common/http";
import { PagedOrderPayment } from "../interfaces/paged-order-payment.interface";

@Injectable({
    providedIn: 'root',
})
export class OrderPaymentService {

    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);

    getOrderPayments(page: number = 1, pageSize: number = 10): Observable<PagedOrderPayment> {
        const url = `${this.apiUrl}/OrderPayments/?page=${page}&pageSize=${pageSize}`;
        return this.http.get<PagedOrderPayment>(url);
    }
}
