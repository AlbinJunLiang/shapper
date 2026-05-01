import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { PagedFeaturedProduct } from '../interfaces/paged-featured-product.interafce';
import { Observable } from 'rxjs';
import { FeaturedProductResponse } from '../interfaces/featured-product-response.interface';

@Injectable({
  providedIn: 'root',
})
export class FeaturedProductService {

  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getFeaturedProducts(page: number = 1, pageSize: number = 10): Observable<PagedFeaturedProduct> {
    const url = `${this.apiUrl}/FeaturedProducts/?page=${page}&pageSize=${pageSize}`;
    return this.http.get<PagedFeaturedProduct>(url);
  }


  featureProduct(productId: number): Observable<{ success: boolean; message: string; data: FeaturedProductResponse }> {
    const url = `${this.apiUrl}/FeaturedProducts`;
    const body = { productId };
    return this.http.post<{ success: boolean; message: string; data: FeaturedProductResponse }>(url, body);
  }

  deleteFeaturedProduct(productId: number): Observable<{ success: boolean; message: string }> {
    const url = `${this.apiUrl}/FeaturedProducts/${productId}`;

    return this.http.delete<{ success: boolean; message: string }>(url);
  }

}
