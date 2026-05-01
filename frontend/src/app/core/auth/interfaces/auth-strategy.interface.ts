import { Observable } from 'rxjs';
import { AuthUser } from './auth-user.interface';

export interface IAuthStrategy {
    login(email: string, pass: string): Observable<AuthUser>;
    register(email: string, pass: string, displayName?: string): Observable<AuthUser>;
    logout(): Observable<void>;
    getCurrentUser(): Observable<AuthUser | null>;
    onAuthStateChange(): Observable<AuthUser | null>;
    loginWithProvider(): Observable<void>;
    handleRedirectResult(): Observable<AuthUser | null>;
    getIdToken(): Promise<string | null>;
    sendEmailVerification(): Observable<void>;
    refreshAuthStatus(): Observable<boolean>;
    sendPasswordResetEmail(email: string): Observable<void>;
    deleteAccount(): Observable<void>;
}