import { Injectable, signal, computed, inject, runInInjectionContext, Injector } from '@angular/core';
import { UserSyncResponse } from '../interfaces/user-sync-response.interface';
import { AuthService } from '../auth/services/auth-service';
import { UserService } from './user-service';
import { toObservable } from '@angular/core/rxjs-interop';
import { CustomerDataUpdate } from '../interfaces/customer-update-data.interfaces';
import { lastValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { Role } from '../enums/role.enum';


@Injectable({ providedIn: 'root' })
export class UserStore {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private injector = inject(Injector);
  public isVerified = computed(() => this._userSyncData()?.data.status === 'VERIFIED');
  private _isLoading = signal<boolean>(false);
  public isLoading = this._isLoading.asReadonly();
  private router = inject(Router); // 3. Inyectar aquí

  private _userSyncData = signal<UserSyncResponse | null>(null);
  public userSyncData = computed(() => this._userSyncData());

  // Este método centraliza la lógica que antes estaba en app.ts
  initSync() {
    runInInjectionContext(this.injector, () => {
      toObservable(this.authService.user).subscribe(user => {
        if (!user) {
          this.clearUserData();
          return;
        }

        this.userService.createUser(user.displayName ?? 'User', '')
          .subscribe({
            next: (resp) => {
              this._userSyncData.set(resp);

              const currentUrl = this.router.url;
              const isAtGateway = currentUrl === '/' || currentUrl === '/home' || currentUrl === '';

              if (isAtGateway && resp.data) {
                if (resp.data.roleId === Role.ADMIN) {
                  this.router.navigate(['/admin'], { replaceUrl: true });
                }
              }
            },
            error: (err) => console.error('Error en Store Sync:', err)
          });
      });
    });
  }
  public async setUserSyncData(updateCustomerData: CustomerDataUpdate): Promise<boolean> {
    const userData = this._userSyncData()?.data;
    if (!userData) return false;

    this._isLoading.set(true);
    try {
      // lastValueFrom convierte el Observable en una Promesa
      const resp = await lastValueFrom(this.userService.updateUserName(userData.email, updateCustomerData));

      this._userSyncData.set({
        message: 'User updated successfully',
        isNew: false,
        data: { ...userData, ...resp }
      });

      return true; // Éxito
    } catch (err) {

      return false; // Error
    } finally {
      this._isLoading.set(false); // Se ejecuta siempre (éxito o error)
    }
  }

  // Método privado para resetear los datos
  private clearUserData() {
    this._userSyncData.set(null);
  }

  public getUserName() {
    const user = this.userSyncData()?.data;

    if (!user) return '';
    const name = user.name ?? '';
    const lastName = user.lastName ?? '';
    return `${name} ${lastName}`.trim();
  }

}