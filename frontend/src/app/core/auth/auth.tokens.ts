// tokens/auth.tokens.ts
import { InjectionToken } from '@angular/core';
import { IAuthStrategy } from './interfaces/auth-strategy.interface';

export const AUTH_STRATEGY = new InjectionToken<IAuthStrategy>('AUTH_STRATEGY');