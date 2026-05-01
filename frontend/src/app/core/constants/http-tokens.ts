// src/app/core/constants/http-context.ts
import { HttpContextToken } from '@angular/common/http';

export const IS_PUBLIC = new HttpContextToken<boolean>(() => false);