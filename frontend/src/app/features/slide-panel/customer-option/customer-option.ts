import { Component, inject } from '@angular/core';
import { ToggleSlider } from '../../../core/services/toggle-slider';
import { CartStorage } from '../../../core/services/cart-storage';
import { NotificationService } from '../../../core/services/notification.service';
import { AuthService } from '../../../core/auth/services/auth-service';
import { MatIcon } from "@angular/material/icon";
import { DialogService } from '../../../core/services/dialog.service';
import { CategoryStore } from '../../../core/services/category-store';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-customer-option',
  imports: [MatIcon, TranslateModule],
  templateUrl: './customer-option.html',
  styleUrl: './customer-option.css',
})
export class CustomerOption {
  protected showSlidePanel = inject(ToggleSlider);
  private notify = inject(NotificationService);
  protected cartStorage = inject(CartStorage);
  protected authService = inject(AuthService);
  protected categoryStore = inject(CategoryStore);
  readonly dialogService = inject(DialogService);


  resetCart() {
    if (this.cartStorage.totalUnits() > 0) {
      this.cartStorage.clearCart()
      this.showSlidePanel.toggle();
    } else {
      this.notify.show('CART.EMPTY', 'ACTIONS.CLOSE');
    }
  }

  openCartDialog() {
    if (this.cartStorage.totalUnits() > 0) {
      this.dialogService.openCartDialogService();
    } else {
      this.notify.show('CART.EMPTY', 'ACTIONS.CLOSE');
    }
  }

  openFilterDialog() {
    if (this.categoryStore.totalCategories()>0) {
      this.dialogService.openFilterDialog();
    } else {
      this.notify.show('COMMON.NO_FILTERS', 'ACTIONS.CLOSE');
    }
  }
  
}
