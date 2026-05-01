import { Component, computed, inject, signal, viewChild, ViewChild } from '@angular/core';
import { ProductDialog } from '../product-dialog/product-dialog';
import { MatDialogRef, MatDialogClose, MatDialog } from '@angular/material/dialog';
import { ProductAddedCard } from "./product-added-card/product-added-card";
import { MatTableModule } from "@angular/material/table";
import { MatTab, MatTabGroup } from '@angular/material/tabs';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CheckoutForm } from "./checkout-form/checkout-form";
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OrderPaymentCard } from "./order-payment-card/order-payment-card";
import { PaymentMethodType } from '../../core/types/payment-method.type';
import { CartStorage } from '../../core/services/cart-storage';
import { LocationStore } from '../../core/services/location-store';
import { ProductAdded } from '../../core/interfaces/product-added.interface';
import { StoreService } from '../../core/services/store-service';
import { OrderService } from '../../core/services/order-service';
import { UserStore } from '../../core/services/user-store';
import { NotificationService } from '../../core/services/notification.service';
import { PaymentService } from '../../core/services/payment-service';
import { VerifyDialog } from '../dialog/verify-dialog/verify-dialog';
import { LinkType } from '../../core/enums/link-type.enum';
import { LinkName } from '../../core/enums/link-name.enum';
import { MatIcon } from "@angular/material/icon";
import { TranslateModule } from '@ngx-translate/core';
import { environment } from '../../../environments/environment.development';

@Component({
  selector: 'app-shopping-cart-dialog',
  imports: [MatProgressSpinnerModule, MatDialogClose, ProductAddedCard,
    MatTableModule, MatTab, MatTabGroup, MatFormFieldModule, MatInputModule,
    CheckoutForm, OrderPaymentCard, MatIcon, TranslateModule],
  templateUrl: './shopping-cart-dialog.html',
  styleUrl: './shopping-cart-dialog.css',
})
export class ShoppingCartDialog {

  @ViewChild(OrderPaymentCard) orderPaymentCard!: OrderPaymentCard;

  public cartStorage = inject(CartStorage);
  protected  currency = environment.currency;
  protected dialogRef = inject(MatDialogRef<ProductDialog>);
  protected checkoutFormData: any | null = null;
  protected isPaymentDisabled: boolean = true;
  protected paymentMethodSelected = signal<PaymentMethodType>('none');
  protected isListDisabled: boolean = false;
  protected isVerificationDisabled: boolean = false;
  protected tabIndex = 0;
  protected isLoading = signal(false);
  public isProcessingPayment = signal(false);
  private checkoutForm = viewChild(CheckoutForm);
  private locationStore = inject(LocationStore);
  private storeInformationService = inject(StoreService);
  private orderService = inject(OrderService);
  private paymentService = inject(PaymentService);
  private dialog = inject(MatDialog);
  private userStore = inject(UserStore);
  private notify = inject(NotificationService);
  private cancelUrl = computed(() =>
    this.storeInformationService.getLink(LinkType.PAYMENT, LinkName.CANCEL)?.url || ''
  );

  private sucessUrl = computed(() =>
    this.storeInformationService.getLink(LinkType.PAYMENT, LinkName.SUCCESS)?.url || ''
  );

  private companyName = computed(() =>
    this.storeInformationService.storeData()?.name || 'Loading...'
  );

  public isCartEmpty = computed(() => {
    const units = this.cartStorage.totalUnits();
    return units <= 0;
  });

  public isFormValid = computed(() => {
    const formChild = this.checkoutForm();
    if (!formChild) return false;
    return formChild.isValid();
  });

  public shippingCost = signal(0);

  ngOnInit() {
    this.isLoading.set(true);
    setTimeout(() => {
      this.isLoading.set(false);
    }, 500);
  }

  updateShippingCost(locationId: number) {
    const locations = this.locationStore.locations();
    const place = locations.find(loc => loc.id === locationId);
    if (place) {
      this.shippingCost.set(place.cost);
    }
  }

  closeDialog() {
    this.dialogRef.close();
  }

  nextTab() {
    this.tabIndex++;
    switch (this.tabIndex) {
      case 1:
        this.goStepOne();
        break;
      case 2:
        this.goStepTwo();
        break;
      case 3:
        this.goStepThree();
        break;
    }
  }

  private goStepOne() {
    if (this.isCartEmpty() == false) {
      this.isVerificationDisabled = false;
    } else {
      this.isVerificationDisabled = true;
      this.prevTab();
    }
  }

  private goStepTwo() {
    this.checkoutFormData = this.checkoutForm()?.getData();
    if (this.isVerificationDisabled == false && this.isCartEmpty() == false && this.isFormValid()) {
      this.isPaymentDisabled = false;
    } else {
      this.isPaymentDisabled = true;
      this.prevTab();
    }
  }


  private goStepThree() {
    if (this.isPaymentMethodSelected()) {
      this.isListDisabled = true;
      this.isVerificationDisabled = true;
      this.processingPayment();
    } else {
      this.prevTab();
    }
  }

  prevTab() {
    this.tabIndex--;
  }

  isPaymentMethodSelected() {
    return this.orderPaymentCard.isPaymentMethodSelected()
  }

  updatePaymentMethod(method: PaymentMethodType) {
    this.paymentMethodSelected.set(method);
  }

  processingPayment() {
    let paymentProvider = this.paymentMethodSelected();
    const customerId = this.userStore.userSyncData()?.data.id || null;
    const newOrder = this.mapToCreateOrder(paymentProvider, this.cartStorage.cartItems(), customerId);
    if (paymentProvider == 'whatsapp') {
      this.createOrder(newOrder);
    } else {
      this.initCheckout(newOrder);
    }
  }

  private createOrder(newOrder: any) {
    this.isProcessingPayment.set(true);
    this.orderService.createOrder(newOrder).subscribe({
      next: (response) => {
        this.isProcessingPayment.set(false);
        this.openWhatsApp(response.orderReference);
        this.showMessage();

      },
      error: (err) => {
        this.notify.show("PAYMENT.ERROR");
        this.closeDialog();
      },
      complete: () => {
        this.isProcessingPayment.set(false);
      }
    });
  }

  private initCheckout(newOrder: string) {
    this.isProcessingPayment.set(true);
    this.paymentService.checkoutOrder(newOrder).subscribe({
      next: (response) => {
        window.location.href = response.paymentUrl;
        this.closeDialog();
        this.showMessage();
        this.cartStorage.clearCart();
      },
      error: (err) => {
        this.notify.show("PAYMENT.ERROR");
        this.closeDialog();
      },
      complete: () => {
        this.isProcessingPayment.set(false);
      }
    });
  }


  private openWhatsApp(orderReference: string) {
    const phoneNumber = this.storeInformationService.storeData()?.phoneNumber ?? '';
    const total = this.cartStorage.totalAmount() + this.shippingCost();
    const texto = `Hola. Quiero pagar mi orden de $${total.toFixed(2)}. \nOrden: ${orderReference}`;
    const url = `https://wa.me/${phoneNumber}?text=${encodeURIComponent(texto)}`;
    if (this.isProcessingPayment() == false) {
      this.closeDialog();
      this.cartStorage.clearCart();

    }
    window.open(url, '_blank');
  }


  private mapToCreateOrder(
    paymentProvider: string,
    itemsCart: ProductAdded[],
    customerId: number | null
  ): any {

    const checkoutFormData = this.checkoutForm()?.getData();
    const locationId = Number(checkoutFormData?.place);
    const place = this.locationStore.locations().find(loc => loc.id === locationId)?.name ?? '';

    const mappedItems = itemsCart.map(item => ({
      productId: item.idProduct,
      quantity: item.quantity,
      productImageUrl: item.imageUrl,
      description: item.additionalDetails ?? '',
      price: item.currentPrice
    }));

    let body = {
      ...(customerId ? { customerId } : {}),
      extraData: {
        name: checkoutFormData?.name ?? '',
        lastName: checkoutFormData?.lastName ?? '',
        email: checkoutFormData?.email,
        address: checkoutFormData?.address ?? '',
        phoneNumber: checkoutFormData?.phoneNumber ?? '',
        place: place,
        postalCode: checkoutFormData?.postalCode ?? ''
      },
      provider: paymentProvider,
      companyName: this.companyName(),
      ...(locationId ? { locationId } : {}),
      successUrl: this.sucessUrl(),
      cancelUrl: this.cancelUrl(),
      items: mappedItems
    };
    return body;
  }

  private showMessage() {
    this.dialog.open(VerifyDialog, {
      width: '250px',
      enterAnimationDuration: '0ms',
      exitAnimationDuration: '0ms',
      data: {
        title: 'ORDER.TITLE',
        message: 'ORDER.REGISTERED'
      }
    });
  }
}
