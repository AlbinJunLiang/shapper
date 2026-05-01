import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';

@Injectable({ providedIn: 'root' })
export class NotificationService {
    private _snackBar = inject(MatSnackBar);
    private _translate = inject(TranslateService);
    private translate = inject(TranslateService);

    /**
     * Muestra una alerta traducida
     * @param messageKey Llave del JSON para el mensaje (ej: 'CART.EMPTY')
     * @param actionKey Llave del JSON para el botón (ej: 'ACTIONS.CLOSE')
     */
    show(messageKey: string, actionKey: string = 'ACTIONS.CLOSE') {
        this._translate.get([messageKey, actionKey]).subscribe((res) => {
            this._snackBar.open(res[messageKey], res[actionKey], {
                duration: 3000,
                horizontalPosition: 'center',
                verticalPosition: 'top',
            });
        });
    }


    // Agregamos interpolateParams como tercer argumento
    show2(
        messageKey: string,
        actionKey: string = 'ACTIONS.CLOSE',
        interpolateParams: Object = {} // <--- Nuevo: Para pasar los minutos u otros datos
    ) {
        // Pasamos los parámetros al método get() de translate
        this._translate.get([messageKey, actionKey], interpolateParams).subscribe((res) => {
            this._snackBar.open(res[messageKey], res[actionKey], {
                duration: 3000,
                horizontalPosition: 'center',
                verticalPosition: 'top',
            });
        });
    }
    showWelcome(userName: string) {
        const mensaje = this.translate.instant('AUTH_MESSAGES.WELCOME', {
            name: userName || 'User'
        });

        this.show(mensaje);
    }

}