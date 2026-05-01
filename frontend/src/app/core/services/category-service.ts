import { HttpClient, HttpContext } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { from, Observable, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedCategories } from '../interfaces/paged-categoy.interface';
import { FilterCategoryResponse } from '../interfaces/filter-category-response.interface';
import { IS_PUBLIC } from '../constants/http-tokens';
import { CategoryRequest } from '../interfaces/category-request.interface';
import { convertAnyImageToWebP } from '../shared/utils/image-util';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {

  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getCategories(page: number = 1, pageSize: number = 10): Observable<PagedCategories> {
    const url = `${this.apiUrl}/Categories/?page=${page}&pageSize=${pageSize}`;
    return this.http.get<PagedCategories>(url,  {
      context: new HttpContext().set(IS_PUBLIC, true)
    });
  }

  getCategoriesWithPriceRange() {
    const url = `${this.apiUrl}/Categories/with-price-range`;
    return this.http.get<FilterCategoryResponse>(url, {
      context: new HttpContext().set(IS_PUBLIC, true)
    });
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Categories/${id}`);
  }


  createCategory(
    data: CategoryRequest,
    imageFile?: File
  ): Observable<any> {
    const url = `${this.apiUrl}/Categories`;

    const formData = new FormData();
    formData.append('Name', data.name);
    formData.append('Description', data.description ?? '');
    formData.append('ImageProvider', data.imageProvider ?? 'LOCAL');

    if (imageFile) {
      return from(convertAnyImageToWebP(imageFile, 0.8)).pipe(
        switchMap(({ blob }) => {
          const newName = imageFile.name.replace(/\.\w+$/, '.webp');
          formData.append('ImageFile', blob, newName);
          return this.http.post(url, formData);
        })
      );
    } else {
      formData.append('ImageUrl', data.imageUrl ?? '');
      return this.http.post(url, formData);
    }
  }


  updateCategory(
    categoryId: number,
    data: CategoryRequest,
    imageFile?: File
  ): Observable<any> {
    const url = `${this.apiUrl}/Categories/${categoryId}`;

    const formData = new FormData();
    formData.append('Name', data.name);
    formData.append('Description', data.description ?? '');
    formData.append('ImageProvider', data.imageProvider ?? 'LOCAL');

    if (imageFile) {
      return from(convertAnyImageToWebP(imageFile, 0.8)).pipe(
        switchMap(({ blob }) => {
          const newName = imageFile.name.replace(/\.\w+$/, '.webp');
          formData.append('ImageFile', blob, newName);
          return this.http.put(url, formData);
        })
      );
    } else {
      formData.append('ImageUrl', data.imageUrl ?? '');
      return this.http.put(url, formData);
    }
  }
}
