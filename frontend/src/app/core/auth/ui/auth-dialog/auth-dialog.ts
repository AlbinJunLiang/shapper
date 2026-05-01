import { Component, inject } from '@angular/core';
import { MatDialogRef, MatDialogClose, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Register } from "./register/register";
import { Login } from "./login/login";
import { AuthDialogType } from './auth-dialog.type';
import { Forgot } from "./forgot/forgot";
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-auth-dialog',
  imports: [MatDialogClose, Register, Login, Forgot, TranslateModule],
  templateUrl: './auth-dialog.html',
  styleUrl: './auth-dialog.css',
})
export class AuthDialog {
  public dialogRef = inject(MatDialogRef<AuthDialog>);
  private dialogData = inject(MAT_DIALOG_DATA);
  private _dialogMode: AuthDialogType = 'login';

  constructor() {
    if (this.dialogData) {
      this._dialogMode = this.dialogData.mode;
    }
  }
  set dialogMode(dialogMode: AuthDialogType) {
    this._dialogMode = dialogMode;
  }

  get dialogMode(): AuthDialogType {
    return this._dialogMode;
  }
}
