import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { from, Observable, switchMap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { convertAnyImageToWebP } from '../shared/utils/image-util';

@Injectable({
    providedIn: 'root'
})
export class ProductImageService {
    private http = inject(HttpClient);
    private readonly apiUrl = environment.apiUrl;

    /**
     * Sube una imagen (archivo o URL) vinculada a un producto
     */
 uploadProductImage(
  productId: number,
  data: { file?: File; imageUrl?: string; provider: string }
): Observable<any> {
  const url = `${this.apiUrl}/ProductImages`;
  const formData = new FormData();
  formData.append('ProductId', productId.toString());

  if (data.file) {
    // 1. Convertimos el archivo a WebP antes de enviarlo
    return from(convertAnyImageToWebP(data.file, 0.8)).pipe(
      switchMap(({ blob }) => {
        // Cambiamos la extensión del nombre original a .webp
        const newName = data.file!.name.replace(/\.\w+$/, '.webp');
        
        formData.append('ImageFile', blob, newName);
        formData.append('Provider', data.provider);
        
        return this.http.post(url, formData);
      })
    );
  } else {
    if (data.imageUrl) {
      formData.append('ImageUrl', data.imageUrl);
    }
    // Nota: Aquí podrías querer enviar el provider también si es necesario
    return this.http.post(url, formData);
  }
}

    deleteProductImage(imageId: number | string, provider: string): Observable<any> {
        const params = new HttpParams().set('provider', provider);
        return this.http.delete(`${this.apiUrl}/ProductImages/${imageId}`, { params });
    }
}