import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CreateLocation, LocationResponse } from '../interfaces/location.interface';
import { Observable } from 'rxjs';
import { PagedLocations } from '../interfaces/paged-location.interface';

@Injectable({
  providedIn: 'root',
})
export class LocationService {
  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getLocations(page: number = 1, pageSize: number = 100): Observable<PagedLocations> {
    const url = `${this.apiUrl}/Locations/?page=${page}&pageSize=${pageSize}`;
    return this.http.get<PagedLocations>(url);
  }

  createLocation(location: CreateLocation): Observable<LocationResponse> {
    return this.http.post<LocationResponse>(`${this.apiUrl}/Locations`, location);
  }

  updateLocation(location: CreateLocation, idLocation: number): Observable<LocationResponse> {
    return this.http.put<LocationResponse>(`${this.apiUrl}/Locations/${idLocation}`, location);
  }

  deleteLocation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Locations/${id}`);
  }
}
