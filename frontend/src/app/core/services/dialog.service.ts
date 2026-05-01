import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../auth/ui/confirm-dialog/verify-dialog';
import { ShoppingCartDialog } from '../../features/shopping-cart-dialog/shopping-cart-dialog';
import { FilterDialog } from '../../features/filter-dialog/filter-dialog';


@Injectable({ providedIn: 'root' })
export class DialogService {
  private dialog = inject(MatDialog);

  openConfirmVerification() {
    if (this.dialog.openDialogs.length > 0) return;
    return this.dialog.open(VerifyDialog, {
      disableClose: true,
      enterAnimationDuration: '0ms',
      exitAnimationDuration: '0ms',
      data: {
        title: 'Verificación de Cuenta',
        message: 'Hemos detectado que tu correo aún no está verificado.'
      }
    });
  }

  openCartDialogService() {
    this.dialog.closeAll();

    this.dialog.open(ShoppingCartDialog, {
      enterAnimationDuration: '100ms',
      exitAnimationDuration: '100ms',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
    });
  }

  openFilterDialog() {
    this.dialog.closeAll();
    this.dialog.open(FilterDialog, {
      enterAnimationDuration: '100ms',
      exitAnimationDuration: '100ms',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
    });
  }
}