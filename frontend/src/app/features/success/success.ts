import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { ActivatedRoute, Router } from "@angular/router";
import { PaymentService } from '../../core/services/payment-service';
import { MatDialog } from '@angular/material/dialog';
import { VerifyDialog } from '../dialog/verify-dialog/verify-dialog';
import { AuthService } from '../../core/auth/services/auth-service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-success',
  imports: [MatIcon, TranslateModule],
  templateUrl: './success.html',
  styleUrl: './success.css',
})
export class Success {

  private route = inject(ActivatedRoute);
  private paymentService = inject(PaymentService);
  private readonly dialog = inject(MatDialog);
  private authService = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      const provider = 'paypal';
      if (token) {
        this.paymentService.confirmPayment(token, provider).subscribe({
          next: (res) => {
            if (res.success == false) {
              this.alreadyDoneMessage();
              this.router.navigate(['/home']);
              return;
            } else {
              this.successMessage();
            }
          },
          error: (err) => {
            this.failedMessage();
          }
        });
      }
    });
  }

  private successMessage() {
    this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'PAYMENT.SUCCESS',
        message: `PAYMENT.REGISTERED`
      }
    });
  }

  private failedMessage() {
    this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'PAYMENT.TITLE',
        message: `PAYMENT.ERROR`
      }
    });
  }

  private alreadyDoneMessage() {
    this.dialog.open(VerifyDialog, {
      width: '350px',
      data: {
        title: 'PAYMENT.SUCCESS',
        message: 'PAYMENT.ALREADY_DONE'
      }
    });
  }
  protected goToOrder() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/orders']);
    } else {
      this.dialog.open(VerifyDialog, {
        width: '350px',
        data: {
          title: 'PAYMENT.TITLE',
          message: `PAYMENT.ONLY_REGISTERED_USERS`
        }
      });
    }
  }
}
