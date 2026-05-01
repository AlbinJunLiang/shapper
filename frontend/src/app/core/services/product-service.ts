import { Injectable, inject } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { Product } from '../interfaces/product.interface';
import { ProductRequest } from '../interfaces/product-request.interface';
import { IS_PUBLIC } from '../constants/http-tokens';

@Injectable({
  providedIn: 'root',
})
export class ProductService {

  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  createProduct(product: ProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/Products`, product);
  }


  updateProduct(product: ProductRequest, productId: number): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/Products/${productId}`, product);
  }

  getProductsStore(page: number = 1, pageSize: number = 10, onlyFeatured: boolean = false) {
    return this.http.get<any>(`${this.apiUrl}/Products/store?page=${page}&pageSize=${pageSize}&featured=${onlyFeatured}`, {
      context: new HttpContext().set(IS_PUBLIC, true)
    })
      .pipe(
        catchError((err) => throwError(() => err))
      );
  }

  getProductsAdmin(page: number = 1, pageSize: number = 10, onlyFeatured: boolean = false) {
    return this.http.get<any>(`${this.apiUrl}/Products?page=${page}&pageSize=${pageSize}&featured=${onlyFeatured}`)
      .pipe(
        catchError((err) => throwError(() => err))
      );
  }

  filterProducts(filters: any): Observable<any> {
    const params = new HttpParams({ fromObject: filters });

    return this.http.get<any>(`${this.apiUrl}/Products/filter`, {
      params,
      context: new HttpContext().set(IS_PUBLIC, true)
    }).pipe(
      catchError((err) => throwError(() => err))
    );
  }


  searchProducts(term: string, count: number = 5) {
    const url = `${this.apiUrl}/Products/search?term=${term}&count=${count}`;

    return this.http.get<Product[]>(url, {
      context: new HttpContext().set(IS_PUBLIC, true)
    }).pipe(
      catchError((err) => throwError(() => err))
    );
  }

  getProduct(idProduct: number): Observable<Product> {
    const url = `${this.apiUrl}/Products/${idProduct}`;

    return this.http.get<Product>(url, {
      context: new HttpContext().set(IS_PUBLIC, true)
    }).pipe(
      catchError((err) => throwError(() => err))
    );
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Products/${id}`);
  }
}