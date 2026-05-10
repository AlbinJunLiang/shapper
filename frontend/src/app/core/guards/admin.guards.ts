import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { UserStore } from '../services/user-store';
import { Role } from '../enums/role.enum';
import { toObservable } from '@angular/core/rxjs-interop'; // Importante
import { map, take } from 'rxjs';


export const adminGuard: CanActivateFn = () => {
    const userStore = inject(UserStore);
    const router = inject(Router);

    return toObservable(userStore.userSyncData).pipe(
        take(1),
        map(user => {
            if (user?.data.roleId === Role.ADMIN) {
                return true;
            }

            return router.parseUrl('/home');
        })
    );
};