import { Component, inject, output, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthDialogType } from '../auth-dialog.type';
import { MatIcon } from "@angular/material/icon";
import { AuthService } from '../../../services/auth-service';
import { NotificationService } from '../../../../services/notification.service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { TimerService } from '../../../services/time-service-forgot';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-forgot',
  standalone: true,
  imports: [ReactiveFormsModule, MatIcon, MatProgressSpinner, TranslateModule],
  templateUrl: './forgot.html'
})
export class Forgot {
  private fb = inject(FormBuilder);
  private notifyService = inject(NotificationService);
  protected authService = inject(AuthService);
  protected timerService = inject(TimerService);
  isLoading = signal(false);
  dialogMode = output<AuthDialogType>();

  public form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  sendForgot() {
    if (this.form.valid) {
      const { email } = this.form.value;
      this.onForgotPassword(email); // Llamamos al servicio
    } else {
      this.form.markAllAsTouched();
    }
  }
  onForgotPassword(email: string) {
    this.isLoading.set(true);

    this.authService.sendPasswordResetEmail(email).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.timerService.start();
        this.notifyService.show('Se ha enviado el enlace de recuperación.');
      },
      error: (err) => {
        this.isLoading.set(false);
        this.notifyService.show('No se pudo enviar el enlace.');
      }
    });
  }

  onBackToLogin() {
    this.dialogMode.emit('login');
  }
}