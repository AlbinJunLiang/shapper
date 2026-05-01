import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ReviewResponse } from '../interfaces/review-response.interface';
import { Review } from '../interfaces/review.interface';
import { FilterReviewResponse } from '../interfaces/filter-review-response.interface';
import { ReviewUpdateResponse } from '../interfaces/review-update-response.interface';



@Injectable({
    providedIn: 'root'
})
export class ReviewService {
    private apiUrl = `${environment.apiUrl}/reviews`;

    private http = inject(HttpClient);

    createReview(dto: Review): Observable<ReviewResponse> {
        return this.http.post<ReviewResponse>(this.apiUrl, dto);
    }

    getFilteredReviews(
        productId: number,
        userId?: number,
        sortBy?: string,
        page: number = 1,
        pageSize: number = 10
    ): Observable<FilterReviewResponse> {

        const url = `${this.apiUrl}/filter`;

        let params = new HttpParams()
            .set('productId', productId.toString()) // Ahora va aquí y es obligatorio
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (userId && userId > 0) {
            params = params.set('userId', userId.toString());
        }

        if (sortBy) {
            params = params.set('sortBy', sortBy);
        }

        return this.http.get<FilterReviewResponse>(url, { params });
    }

    // review.service.ts
    updateReview(id: number, review: Review): Observable<ReviewUpdateResponse> {
        return this.http.put<ReviewUpdateResponse>(`${this.apiUrl}/${id}`, review);
    }


    deleteReview(reviewId: number): Observable<void> {
        // Ajusta la URL según tu backend
        return this.http.delete<void>(`${this.apiUrl}/${reviewId}`);
    }

}