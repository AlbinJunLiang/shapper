import { inject, Injectable, signal } from '@angular/core';
import { LocationService } from './location-service';
import { LocationResponse } from '../interfaces/location.interface';

@Injectable({ providedIn: 'root' })
export class LocationStore {
  private locationService = inject(LocationService);

  // Estados privados
  private _locations = signal<LocationResponse[]>([]);
  private _isLoading = signal<boolean>(false);

  // Signals públicas (Solo lectura)
  public locations = this._locations.asReadonly();
  public isLoading = this._isLoading.asReadonly();

  public loadLocations(page: number = 1): void {
    if (this._locations().length > 0) return; // Evita recargar si ya hay datos

    this._isLoading.set(true);
    this.locationService.getLocations(page, 100).subscribe({
      next: (response) => {
        this._locations.set(response.data);
        this._isLoading.set(false);
      },
      error: () => this._isLoading.set(false)
    });
  }
}