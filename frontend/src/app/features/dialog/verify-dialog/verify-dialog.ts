
import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import {
  MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef,
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';


@Component({
  selector: 'app-verify-dialog',
  imports: [MatButtonModule, MatDialogModule, TranslateModule],
  templateUrl: './verify-dialog.html',
  styleUrl: './verify-dialog.css',
})
export class VerifyDialog {
  public data = inject(MAT_DIALOG_DATA);
  readonly dialogRef = inject(MatDialogRef<VerifyDialog>);

}
