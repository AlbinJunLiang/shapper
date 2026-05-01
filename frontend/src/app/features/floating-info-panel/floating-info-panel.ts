import { Component, inject } from '@angular/core';
import { CartStorage } from '../../core/services/cart-storage';
import { NotificationService } from '../../core/services/notification.service';
import { DialogService } from '../../core/services/dialog.service';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-floating-info-panel',
  imports: [TranslateModule],
  templateUrl: './floating-info-panel.html',
  styleUrl: './floating-info-panel.css',
})
export class FloatingInfoPanel {
  protected cartStorage = inject(CartStorage);
  readonly dialogService = inject(DialogService);
  private notify = inject(NotificationService);

  openCartDialog() {
    if (this.cartStorage.totalUnits() > 0) {
      this.dialogService.openCartDialogService();
    } else {
      this.notify.show('CART.EMPTY', 'ACTIONS.CLOSE');
    }
  }

}
