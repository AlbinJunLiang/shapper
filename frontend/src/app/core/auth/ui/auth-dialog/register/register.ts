import { Component, inject, output, signal, viewChild } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { MatIcon } from "@angular/material/icon";
import { matchValidator } from '../../../../shared/validators/password-validator';
import { AuthDialog } from '../auth-dialog';
import { MatDialogRef } from '@angular/material/dialog';
import { AuthService } from '../../../services/auth-service';
import { switchMap } from 'rxjs';
import { NotificationService } from '../../../../services/notification.service';
import { MatProgressSpinner } from "@angular/material/progress-spinner";
import { AuthDialogType } from '../auth-dialog.type';
import { TranslateModule } from '@ngx-translate/core';

export interface RegisterData {
  name: string;
  firstLastName: string;
  secondLastName?: string;
  email: string;
  password?: string;
}

@Component({
  selector: 'app-register',
  imports: [MatButtonModule,
    MatStepperModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule, MatIcon, MatProgressSpinner,
   TranslateModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  isLinear = true;
  hidePassword = signal(true);
  hideConfirmPassword = signal(true);
  dialogMode = output<AuthDialogType>();
  isLoading = signal(false);
  isFirstStepEditable = true;

  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<AuthDialog>);
  private stepper = viewChild.required(MatStepper);
  private notificationService = inject(NotificationService);
  protected authService = inject(AuthService);


  // 1. Definimos los grupos por separado para que el Stepper los entienda
  firstFormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(256)]],
    firstLastName: ['', [Validators.required, Validators.maxLength(256)]],
    secondLastName: ['', [Validators.maxLength(256)]],
  });

  secondFormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required, Validators.minLength(6)]],

    // Este es el de confirmar
  },// En el objeto de opciones del grupo
    { validators: matchValidator });


    
  goingBackToRegister() {
    this.dialogMode.emit('login');
  }

  togglePassword() {
    this.hidePassword.update(v => !v);
  }

  toggleConfirm() {
    this.hideConfirmPassword.update(v => !v);
  }

  close() {
    this.dialogRef.close();
  }

  getFinalData(): RegisterData {
    const data = {
      ...this.firstFormGroup.value,
      ...this.secondFormGroup.value
    };
    delete data.confirmPassword;
    return data as RegisterData;
  }

  register() {
    if (this.secondFormGroup.valid) {
      this.isLoading.set(true);

      // 1. Extraemos los valores (con ! para asegurar que no son null)
      const { name, firstLastName, secondLastName } = this.firstFormGroup.value;
      const { email, password } = this.secondFormGroup.value;

      // 2. Concatenamos el nombre
      const fullName = [name, firstLastName, secondLastName]
        .filter(part => part && part.trim().length > 0)
        .join(' ');

      // 3. Flujo Reactivo: Registro -> Verificación
      this.authService.register(email!, password!, fullName)
        .pipe(
          // switchMap "cambia" el flujo: si el registro es exitoso, intenta enviar el correo
          switchMap(() => this.authService.sendEmailVerification())
        )
        .subscribe({
          next: () => {
            this.isLoading.set(false);
            this.authService.updateDisplayName(fullName);
            this.stepper().next();
            this.isFirstStepEditable = false;
          },
          error: (err) => {
            this.isLoading.set(false);
            this.notificationService.show('El correo ya se encuentra registrado.');
          }
        });

    } else {
      this.secondFormGroup.markAllAsTouched();
    }
  }
  goToFirstStep() {
    if (!this.isLoading()) {
      this.stepper().selectedIndex = 0;
    }
  }
}
