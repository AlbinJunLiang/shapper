import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserStore } from '../services/user-store';
import { Role } from '../enums/role.enum';
import { toObservable } from '@angular/core/rxjs-interop'; // Importante
import { filter, map, take } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
    const userStore = inject(UserStore);
    const router = inject(Router);

    // Convertimos el Signal a un flujo de datos (Observable)
    return toObservable(userStore.userSyncData).pipe(
        // 1. FILTRO: No dejes pasar nada que sea nulo o indefinido
        filter(user => !!user), 
        // 2. TOMAR UNO: En cuanto llegue el primer dato real, deja de escuchar
        take(1),
        // 3. MAPEO: Ahora que tenemos el JSON, aplicamos tu lógica
        map(user => {            
            if (user?.data.roleId === Role.ADMIN) {
                return true; // Acceso concedido
            }

            return router.parseUrl('/home'); // Acceso denegado
        })
    );
};