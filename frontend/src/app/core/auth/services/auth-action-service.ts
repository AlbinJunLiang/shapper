import { inject, Injectable, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { NotificationService } from '../../services/notification.service';
import { ToggleSlider } from '../../services/toggle-slider';
import { AuthService } from './auth-service';

@Injectable({ providedIn: 'root' })
export class AuthActionService {
    private notify = inject(NotificationService);
    private slidePanel = inject(ToggleSlider);
    public isLoggingIn = signal(false);

    public signInWithGoogle(authService: AuthService) {
        this.isLoggingIn.set(true);
        authService.loginWithProvider().pipe(
            finalize(() => this.isLoggingIn.set(false))
        ).subscribe({
            next: () => {
                const name = authService.user()?.displayName || 'User';
                this.notify.showWelcome(name);
                this.slidePanel.close();
            },
            error: (err) => {
                this.notify.show('Error al iniciar sesión.', 'Cerrar');
            }
        });
    }
}