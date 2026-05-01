import { Component, inject, output, signal } from '@angular/core';
import { MatIcon } from "@angular/material/icon";
import { FormBuilder, FormGroup, Validators, ɵInternalFormsSharedModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { NotificationService } from '../../../../services/notification.service';
import { AuthService } from '../../../services/auth-service';
import { AuthDialog } from '../auth-dialog';
import { AuthActionService } from '../../../services/auth-action-service';
import { AuthDialogType } from '../auth-dialog.type';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  imports: [MatIcon, ɵInternalFormsSharedModule, ReactiveFormsModule, MatProgressSpinner, TranslateModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  dialogMode = output<AuthDialogType>();
  hidePassword = signal(true);
  private fb = inject(FormBuilder);
  public form: FormGroup = this.createForm();
  protected authService = inject(AuthService);
  public authActionService = inject(AuthActionService);
  public errorMessage: string = '';
  private dialogRef = inject(MatDialogRef<AuthDialog>);
  private notificationService = inject(NotificationService);
  isLoading = signal(false);

  private createForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(50)
      ]],
    });
  }

  goToRegister() {
    this.dialogMode.emit('register');
  }

  goToForgot() {
    this.dialogMode.emit('forgot');
  }

  close() {
    this.dialogRef.close();
  }

  onLogin() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    const { email, password } = this.form.value;

    this.authService.login(email, password).subscribe({
      next: (user) => {
        this.isLoading.set(false);
        this.notificationService.showWelcome(user.displayName || "")
        this.close();

      },
      error: (errorMessage: string) => {
        this.isLoading.set(false);
        this.errorMessage = errorMessage;
        this.notificationService.show(errorMessage);
      }
    });
  }

  onLoginWithGoogle() {
    this.authActionService.signInWithGoogle(this.authService);
    this.dialogRef.close();
  }

  togglePassword() {
    this.hidePassword.update(v => !v);
  }
}

