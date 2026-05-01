import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedFaq } from '../interfaces/paged-faq.interface';
import { CreateFaq, IFaq, UpdateFaq } from '../interfaces/faq.interface';
import { IS_PUBLIC } from '../constants/http-tokens';

@Injectable({
    providedIn: 'root',
})
export class FaqService {

    private readonly apiUrl = environment.apiUrl;
    private http = inject(HttpClient);

    getFaqs(page: number = 1, pageSize: number = 10): Observable<PagedFaq> {
        const url = `${this.apiUrl}/Faqs/?page=${page}&pageSize=${pageSize}`;

        return this.http.get<PagedFaq>(url, {
            context: new HttpContext().set(IS_PUBLIC, true)
        });
    }

    create(payload: CreateFaq): Observable<IFaq> {
        return this.http.post<IFaq>(`${this.apiUrl}/Faqs`, payload);
    }

    update(id: number, payload: UpdateFaq): Observable<IFaq> {
        return this.http.put<IFaq>(`${this.apiUrl}/Faqs/${id}`, payload).pipe(
            tap((updatedFaq) => {

            })
        );
    }


    delete(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/Faqs/${id}`);
    }
}
