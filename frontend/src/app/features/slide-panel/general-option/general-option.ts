import { Component, computed, inject, signal } from '@angular/core';
import { AuthActionService } from '../../../core/auth/services/auth-action-service';
import { AuthService } from '../../../core/auth/services/auth-service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { ToggleSlider } from '../../../core/services/toggle-slider';
import { AuthDialog } from '../../../core/auth/ui/auth-dialog/auth-dialog';
import { UserStore } from '../../../core/services/user-store';
import { DialogService } from '../../../core/services/dialog.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-general-option',
  imports: [MatProgressSpinner, TranslateModule],
  templateUrl: './general-option.html'
})
export class GeneralOption {
  protected authActionService = inject(AuthActionService);
  protected authService = inject(AuthService);
  private notify = inject(NotificationService);
  readonly dialog = inject(MatDialog);
  private dialogService = inject(DialogService);
  protected showSlidePanel = inject(ToggleSlider);
  public isLoading = signal(false);
  protected userStore = inject(UserStore);
  public verifyStatus = computed(() => this.userStore.isVerified());

  protected loginWithGoogle() {
    this.authActionService.signInWithGoogle(this.authService);
  }

  protected onLogout() {
    this.authService.logout().subscribe({
      next: (user) => {
        this.notify.show('AUTH_MESSAGES.LOGOUT', 'ACTIONS.CLOSE');
        this.showSlidePanel.goToPage('/home');
        this.showSlidePanel.close();
      }
    });
  }

  protected openAuthDialog(mode: 'login' | 'register') {
    this.dialog.closeAll();
    this.dialog.open(AuthDialog, {
      enterAnimationDuration: '100ms',
      exitAnimationDuration: '100ms',
      panelClass: 'custom-tailwind-dialog',
      backdropClass: 'custom-backdrop',
      data: {
        mode: mode
      }
    });

    this.showSlidePanel.close();
  }

  protected openConfirmDialog() {
    this.dialogService.openConfirmVerification();
  }

  protected checkStatus() {
    this.isLoading.set(true);
    this.authService.refreshAuthStatus().subscribe({
      next: (isVerified) => {
        this.isLoading.set(false);
        if (!isVerified) {
          this.openConfirmDialog();
        } else {
          this.userStore.initSync();
          this.notify.show('AUTH_MESSAGES.VERIFIED_ACCOUNT', 'ACTIONS.CLOSE');
        }
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }
}
