import { inject, Injectable, signal } from "@angular/core";
import { Product } from "../interfaces/product.interface";
import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "../../../environments/environment.development";
import { Observable } from "rxjs";
import { HomeData, ResumeInterface } from "../interfaces/home.interface";

@Injectable({
  providedIn: 'root'
})
export class HomeService {


  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;



  /**
   * Obtiene los datos del StoreFront con paginación
   * @param storeId Identificador de la tienda (ej: 'ST-MANUAL01')
   */
  getHomeData(
    storeId: string,
    page: number = 1,
    pageSize: number = 10,
    categoriesPage: number = 1,
    categoriesPageSize: number = 8,
    featured: boolean = true
  ): Observable<ResumeInterface> { // <--- Declaras que retornas el flujo

    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('featured', featured.toString())
      .set('categoriesPage', categoriesPage.toString())
      .set('categoriesPageSize', categoriesPageSize.toString());

    // Simplemente retornas el get, NO te suscribes aquí
    return this.http.get<ResumeInterface>(`${this.apiUrl}/StoreFront/home-data/${storeId}`, { params });
  }
}