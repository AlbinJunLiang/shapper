import { Component, effect, inject } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field'; import { UserStore } from '../../core/services/user-store';
import { NotificationService } from '../../core/services/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../dialog/verify-dialog/verify-dialog';
import { TimerService } from '../../core/auth/services/time-service-forgot';
import { AuthService } from '../../core/auth/services/auth-service';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomerDataUpdate } from '../../core/interfaces/customer-update-data.interfaces';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-user-info',
  imports: [MatInputModule, MatFormFieldModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './user-info.html'
})
export class UserInfo {

  public userStore = inject(UserStore);
  protected notifyService = inject(NotificationService);
  readonly dialog = inject(MatDialog);
  protected timerService = inject(TimerService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  protected formGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(300)]],
    lastName: ['', [Validators.required, Validators.maxLength(300)]],
    phoneNumber: ['', [Validators.required, Validators.maxLength(15), Validators.pattern('^[0-9]*$')]],
    address: ['', [Validators.required, Validators.maxLength(300)]],
  });

  constructor() {
    effect(() => {
      this.setForm();
    });
  }

  getUserData() {
    return this.userStore.userSyncData()?.data;
  }

  setForm() {
    const user = this.getUserData();
    if (user && this.formGroup.pristine) {
      this.formGroup.patchValue(user);
    }
  }

  onForgotPassword(email: string) {
    this.authService.sendPasswordResetEmail(email).subscribe({
      next: () => {
        this.notifyService.show('USER_INFORMATION.SEND_SUCCESS', 'ACTIONS.CLOSE');
        this.timerService.start();
      },
      error: (err) => {
        this.notifyService.show('USER_INFORMATION.FAILED', 'ACTIONS.CLOSE');
      }
    });
  }

  openDialog(enterAnimationDuration: string, exitAnimationDuration: string): void {

    if (this.timerService.itWasSent()) {
      this.notifyService.show2(
        'USER_INFORMATION.TOO_MANY_REQUESTS',
        'ACTIONS.CLOSE',
        { minutes: this.timerService.countdown() }
      );
      return;
    }

    const dialogRef = this.dialog.open(VerifyDialog, {
      width: '250px',
      enterAnimationDuration,
      exitAnimationDuration,
      data: {
        title: 'USER_INFORMATION.TITLE',
        message: 'USER_INFORMATION.MESSAGE'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const email = this.getUserData()?.email;
        if (email) {
          this.onForgotPassword(email);
        } else {
          this.notifyService.show('USER_INFORMATION.FAILED', 'ACTIONS.CLOSE');
        }
      }
    });
  }

  async updateUser() {
    if (this.formGroup.invalid) {
      this.formGroup.markAllAsTouched();
      return;
    }

    const { name, lastName, phoneNumber, address } = this.formGroup.value;
    if (!name || !lastName) {
      return;
    }
    const updateCustomerData: CustomerDataUpdate = {
      name: name.trim() ?? '',
      lastName: lastName.trim() ?? '',
      phoneNumber: phoneNumber ?? '',
      address: address ?? ''
    };

    const isOk = await this.userStore.setUserSyncData(updateCustomerData);

    if (isOk) {
      this.notifyService.show('USER_INFORMATION.UPDATE_SUCCESS', 'ACTIONS.CLOSE');
    } else {
      this.notifyService.show('USER_INFORMATION.UPDATE_FAILED', 'ACTIONS.CLOSE');
    }
  }
} //END
