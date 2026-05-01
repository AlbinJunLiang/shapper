import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { catchError, Observable, throwError } from 'rxjs';
import { UserSyncResponse } from '../interfaces/user-sync-response.interface';
import { CustomerDataUpdate } from '../interfaces/customer-update-data.interfaces';
import { PagedUsers } from '../interfaces/paged-user.interfaces';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  createUser(name: string, lastName: string): Observable<UserSyncResponse> {
    const body = { name, lastName };

    return this.http.post<UserSyncResponse>(`${this.apiUrl}/Users`, body)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  updateUserName(email: string, dto: CustomerDataUpdate): Observable<CustomerDataUpdate> {
    const safeEmail = encodeURIComponent(email);
    return this.http.patch<CustomerDataUpdate>(`${this.apiUrl}/users/customer/${safeEmail}`, dto)
      .pipe(
        catchError((err) => throwError(() => err))
      );
  }
  /*
    * Actualiza únicamente el estado del usuario
    * @param email El correo del usuario a modificar
    * @param newStatus El nuevo estado(ACTIVE, BANNED, etc.)
    */
  updateUserStatus(email: string, newStatus: string): Observable<any> {
    const safeEmail = encodeURIComponent(email);
    const dto = { status: newStatus.toUpperCase().trim() };

    return this.http.patch<any>(`${this.apiUrl}/Users/${safeEmail}`, dto)
      .pipe(
        catchError((err) => {
          return throwError(() => err);
        })
      );
  }

  getUsers(page: number = 1, pageSize: number = 100): Observable<PagedUsers> {
    const url = `${this.apiUrl}/Users/?page=${page}&pageSize=${pageSize}`;
    return this.http.get<PagedUsers>(url);
  }
}