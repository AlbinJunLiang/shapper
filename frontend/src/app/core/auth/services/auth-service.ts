// auth.service.ts
import { inject, Injectable, computed, signal, OnDestroy } from '@angular/core';
import { AUTH_STRATEGY } from '../auth.tokens';
import { AuthUser } from '../interfaces/auth-user.interface';
import { catchError, Observable, Subscription, switchMap, throwError } from 'rxjs';


@Injectable({
    providedIn: 'root'
})

export class AuthService implements OnDestroy {
    private strategy = inject(AUTH_STRATEGY);

    private _user = signal<AuthUser | null>(null);
    private _initializing = signal<boolean>(true);
    private authSubscription: Subscription | null = null;

    public user = computed(() => this._user());
    public isLoggedIn = computed(() => !!this._user());
    public isInitializing = computed(() => this._initializing());

    private _token = signal<string | null>(null);
    public token = computed(() => this._token());

    constructor() {
        this.authSubscription = this.strategy.onAuthStateChange().subscribe({
            next: async (user) => {
                this._user.set(user);

                if (user) {
                    const token = await this.strategy.getIdToken();
                    this._token.set(token);
                } else {
                    this._token.set(null);
                }

                this._initializing.set(false);
            }
        });
    }

    ngOnDestroy() {
        this.authSubscription?.unsubscribe();
    }


    loginWithProvider(): Observable<void> {
        return this.strategy.loginWithProvider().pipe(
            catchError((err) => {
                return throwError(() => err);
            })
        );
    }

    register(email: string, pass: string, name: string) {
        return this.strategy.register(email, pass, name);
    }

    login(email: string, pass: string) {
        return this.strategy.login(email, pass);
    }

    logout() {
        return this.strategy.logout();
    }

    async getTokenAsync(): Promise<string | null> {
        const user = this._user();
        if (!user) return null;
        return await this.strategy.getIdToken();
    }

    sendEmailVerification(): Observable<void> {
        return this.strategy.sendEmailVerification().pipe(
            catchError((err) => {
                return throwError(() => err);
            })
        );
    }

    refreshAuthStatus(): Observable<boolean> {
        return this.strategy.refreshAuthStatus().pipe(
            switchMap(async (isVerified) => {
                if (isVerified) {
                    const newToken = await this.strategy.getIdToken();
                    this._token.set(newToken);
                }
                return isVerified;
            }),
            catchError((err) => {
                console.error('Error en el refresh del servicio:', err);
                return throwError(() => err);
            })
        );
    }

    sendPasswordResetEmail(email: string): Observable<void> {
        return this.strategy.sendPasswordResetEmail(email);
    }

    deleteAccount(): Observable<void> {
        return this.strategy.deleteAccount();
    }

    public updateDisplayName(newName: string) {
        this._user.update(currentUser => {
            if (!currentUser) return null;
            const updatedUser = { ...currentUser, displayName: newName };
            return updatedUser;
        });
    }
}