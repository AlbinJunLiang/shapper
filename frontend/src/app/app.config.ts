import { ApplicationConfig, importProvidersFrom, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { routes } from './app.routes';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { provideTranslateHttpLoader, TranslateHttpLoader } from '@ngx-translate/http-loader';
import { CartStorage } from './core/services/cart-storage';
import { provideFirebaseApp, initializeApp } from '@angular/fire/app';
import { provideAuth, getAuth } from '@angular/fire/auth';
import { environment } from '../environments/environment.development';
import { AuthFirebaseStrategyService } from './core/auth/strategies/auth-firebase-strategy.service';
import { AUTH_STRATEGY } from './core/auth/auth.tokens';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    provideFirebaseApp(() => initializeApp(environment.firebaseConfig)),
    provideAuth(() => {
      const auth = getAuth();
      return auth;
    }),

    {
      provide: AUTH_STRATEGY,
      useClass: AuthFirebaseStrategyService
    },

    provideBrowserGlobalErrorListeners(),
    CartStorage,
    provideRouter(routes, withComponentInputBinding()),

    ...provideTranslateHttpLoader({
      prefix: './i18n/',
      suffix: '.json'
    }),

    importProvidersFrom(
      TranslateModule.forRoot({
        loader: {
          provide: TranslateLoader,
          useClass: TranslateHttpLoader
        }
      })
    ),
  ]
};