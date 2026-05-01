import { from, switchMap } from "rxjs";
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../auth/services/auth-service';
import { inject } from '@angular/core';
import { IS_PUBLIC } from "../constants/http-tokens";


export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  
  // 1. Verificamos la etiqueta en el contexto
  const isPublic = req.context.get(IS_PUBLIC);

  // 2. Si es pública, dejamos pasar la petición tal cual
  if (isPublic) {
    return next(req);
  }

  // 3. Si es privada (por defecto), buscamos el token y clonamos
  return from(authService.getTokenAsync()).pipe(
    switchMap(token => {
      if (token) {
        const authReq = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
        return next(authReq);
      }
      return next(req);
    })
  );
};