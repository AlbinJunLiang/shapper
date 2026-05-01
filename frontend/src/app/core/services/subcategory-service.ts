import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { catchError, from, Observable, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedCategories } from '../interfaces/paged-categoy.interface';
import { SubcategoryRequest } from '../interfaces/subcategory-request.interface';
import { convertAnyImageToWebP } from '../shared/utils/image-util';

@Injectable({
  providedIn: 'root',
})
export class SubcategoryService {

  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getSubcategories(page: number = 1, pageSize: number = 10): Observable<PagedCategories> {
    const url = `${this.apiUrl}/Subcategories/?page=${page}&pageSize=${pageSize}`;
    return this.http.get<PagedCategories>(url);
  }


  deleteSubcategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Subcategories/${id}`);
  }


  createSubcategory(
    data: SubcategoryRequest,
    imageFile?: File
  ): Observable<any> {
    const url = `${this.apiUrl}/Subcategories`;

    const formData = new FormData();
    formData.append('Name', data.name);
    formData.append('CategoryId', data.categoryId.toString());
    formData.append('Description', data.description ?? '');
    formData.append('ImageProvider', data.imageProvider ?? 'LOCAL');

    if (imageFile) {
      return from(convertAnyImageToWebP(imageFile, 0.8)).pipe(
        switchMap(({ blob }) => {
          const newName = imageFile.name.replace(/\.\w+$/, '.webp');
          formData.append('ImageFile', blob, newName);
          return this.http.post(url, formData);
        }),
        catchError(() => {
          // fallback → envía imagen original
          formData.append('ImageFile', imageFile, imageFile.name);
          return this.http.post(url, formData);
        })
      );
    } else {
      formData.append('ImageUrl', data.imageUrl ?? '');
      return this.http.post(url, formData);
    }
  }


  updateSubcategory(
    subcategory: number,
    data: SubcategoryRequest,
    imageFile?: File
  ) {
    const url = `${this.apiUrl}/Subcategories/${subcategory}`;

    const formData = new FormData();
    formData.append('Name', data.name);
    formData.append('Description', data.description ?? '');
    formData.append('CategoryId', data.categoryId.toString());
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
